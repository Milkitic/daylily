using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using Daylily.Common.IO;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;
using Daylily.Common.Utils;
using Daylily.Common.Utils.LogUtils;

namespace Daylily.Common.Function.Application.Command
{
    [Name("深夜美食")]
    [Author("yf_extension")]
    [Version(0, 1, 3, PluginVersion.Beta)]
    [Help("查询深夜美食，若参数为空，则返回随机")]
    [Command("food")]
    public class Food : CommandApp
    {
        [Help("禁用指定相册。")]
        [Arg("disable", Default = 0)]
        public int EnabledAlbumId { get; set; }

        [Help("启用指定相册。")]
        [Arg("enable", Default = 0)]
        public int DisabledAlbumId { get; set; }

        [Help("点赞指定相册，将会以最佳推荐。")]
        [Arg("点赞", Default = 0)]
        public int Like { get; set; }

        [FreeArg]
        public string FoodName { get; set; }

        [Help("若启用，则推荐热门美食。")]
        [Arg("热门", IsSwitch = true)]
        public bool Hot { get; set; }

        [Help("清除并重新建立目录缓存。")]
        [Arg("cc", IsSwitch = true)]
        public bool ClearCache { get; set; }

        public static ConcurrentDictionary<string, List<string>> LikeDic { get; set; }

        private static string _imagePath, _content;
        private CommonMessage _cm;

        public override void Initialize(string[] args)
        {
            _imagePath = Path.Combine(SettingsPath, "image");
            _content = Path.Combine(_imagePath, ".content");
            SaveSettings(this);
            if (!File.Exists(_content))
            {
                Logger.Info("正在建立目录……");
                CreateContent();
            }
            else
                Logger.Info("目录已经建立。");

            LikeDic = LoadSettings<ConcurrentDictionary<string, List<string>>>() ??
                      new ConcurrentDictionary<string, List<string>>();

            Logger.Info("已载入点赞情况。");

        }

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            _cm = messageObj;
            string[] fullContent = ConcurrentFile.ReadAllLines(_content);

            if (EnabledAlbumId > 0 || DisabledAlbumId > 0)
            {
                return _cm.PermissionLevel == PermissionLevel.Root
                    ? ManageAlbum(fullContent)
                    : new CommonMessageResponse(LoliReply.RootOnly, _cm);
            }

            if (Like > 0)
            {
                string[] album = EnumerateAlbumByNum(Like, fullContent).ToArray();

                if (ValidateCount(album, out var response))
                    return response;

                if (!LikeDic.ContainsKey(_cm.UserId))
                    LikeDic.TryAdd(_cm.UserId, new List<string>());

                if (LikeDic[_cm.UserId].Contains(album[0]))
                    return new CommonMessageResponse("你点过赞啦", _cm, true);

                LikeDic[_cm.UserId].Add(album[0]);
                SaveSettings(LikeDic);
                return new CommonMessageResponse($"已点赞 \"{Like}\"", _cm);
            }

            if (Hot)
            {
                Dictionary<string, int> hotFoods = new Dictionary<string, int>();
                foreach (var item in LikeDic)
                {
                    foreach (var i in item.Value)
                    {
                        if (hotFoods.ContainsKey(i))
                            hotFoods[i]++;
                        else
                            hotFoods.Add(i, 1);
                    }
                }

                string[] ok = hotFoods.Where(c => c.Value > 1).OrderByDescending(c => c.Value).Select(c => c.Key).ToArray();
                if (ok.Length < 1)
                    return new CommonMessageResponse("目前没有热门相册…", _cm);
                int ubound = ok.Length > 10 ? 10 : ok.Length;
                string hot = ok[Rnd.Next(ubound)];
                string hotPath = Path.Combine(_imagePath, hot);
                var hotFile = GetRandomPhoto(hotPath);

                Bitmap hotBitmap = DrawWatermark(hotFile);
                return new CommonMessageResponse(new FileImage(hotBitmap, 85).ToString(), _cm);
            }

            if (ClearCache)
            {
                ClearContent();
                return new CommonMessageResponse("已重新建立缓存", _cm);
            }

            string[] choices = int.TryParse(FoodName, out _)
                ? EnumerateAlbumByNum(int.Parse(FoodName), fullContent).ToArray()
                : EnumerateAlbumBySearch(FoodName, fullContent).ToArray();

            if (choices.Length == 0 && FoodName != null)
                return new CommonMessageResponse(
                    string.Format("没有找到 \"{0}\"", FoodName.Length > 30 ? FoodName.Substring(0, 27) + "…" : FoodName),
                    _cm);

            var dir = GetRandomAlbum(choices);

            var file = GetRandomPhoto(dir);

            Bitmap bitmap = DrawWatermark(file);
            return new CommonMessageResponse(new FileImage(bitmap, 85).ToString(), _cm);
        }

        private static string GetRandomPhoto(string dir)
        {
            string[] innerContent = ConcurrentFile.ReadAllLines(Path.Combine(dir, ".content"));
            string innerChoice = innerContent[Rnd.Next(0, innerContent.Length)];
            string file = Path.Combine(dir, innerChoice);
            return file;
        }

        private static string GetRandomAlbum(string[] albumName)
        {
            string choice = albumName[Rnd.Next(0, albumName.Length)];
            string dir = Path.Combine(_imagePath, choice);
            return dir;
        }

        private CommonMessageResponse ManageAlbum(IEnumerable<string> content)
        {
            string disabledPath = Path.Combine(_imagePath, ".disabled");
            if (!Directory.Exists(disabledPath))
                Directory.CreateDirectory(disabledPath);
            string[] album;
            string sourcePath, targetPath;
            string message;
            string name;
            if (EnabledAlbumId > 0)
            {
                album = EnumerateAlbumByNum(EnabledAlbumId, content).ToArray();

                name = album[0];
                sourcePath = Path.Combine(_imagePath, name);
                targetPath = disabledPath;
                message = $"成功禁用 \"{album[0]}\"";
            }
            else
            {
                DirectoryInfo dirInfo = new DirectoryInfo(disabledPath);
                string[] disContent = dirInfo.EnumerateDirectories().Select(i => i.Name).ToArray();
                album = EnumerateAlbumByNum(DisabledAlbumId, disContent).ToArray();

                name = album[0];
                sourcePath = Path.Combine(disabledPath, name);
                targetPath = _imagePath;
                message = $"成功启用 \"{album[0]}\"";
            }

            if (ValidateCount(album, out var response))
                return response;

            Directory.Move(sourcePath, Path.Combine(targetPath, name));
            ClearContent();
            return new CommonMessageResponse(message, _cm);
        }
        private static int GetAlbumBySearch(string keyword, IEnumerable<string> content, out string result)
        {
            var resArray = EnumerateAlbumBySearch(keyword, content).ToArray();
            result = resArray.FirstOrDefault();
            return resArray.Length;
        }

        private static int GetAlbumByNum(int albumNum, IEnumerable<string> content, out string result)
        {
            var resArray = EnumerateAlbumByNum(albumNum, content).ToArray();
            result = resArray.FirstOrDefault();
            return resArray.Length;
        }

        private static IEnumerable<string> EnumerateAlbumBySearch(string keyword, IEnumerable<string> content)
        {
            return keyword == null
                ? content
                : content.Where(k => k.Contains(keyword));
        }

        private static IEnumerable<string> EnumerateAlbumByNum(int albumNum, IEnumerable<string> content)
        {
            string strNum = albumNum.ToString();
            return strNum == null
                ? content
                : content.Where(k =>
                    k.Substring(0, k.IndexOf(' ')) == strNum);
        }

        private bool ValidateCount(string[] album, out CommonMessageResponse response)
        {
            if (album.Length > 1)
            {
                response = new CommonMessageResponse($"包含多个相册：\"{string.Join(',', album)}\"", _cm);
                return true;
            }

            if (album.Length == 0)
            {
                response = new CommonMessageResponse($"没有找到相册 \"{EnabledAlbumId}\"", _cm);
                return true;
            }

            response = null;
            return false;
        }

        private static Bitmap DrawWatermark(string path)
        {
            FileInfo fi = new FileInfo(path);
            string mark = fi.Directory.Name;
            Bitmap bmp = new Bitmap(path);
            using (Graphics g = Graphics.FromImage(bmp))
            using (Brush b = new SolidBrush(Color.White))
            using (Pen p = new Pen(Color.FromArgb(96, 0, 0, 0), 2))
            using (Font f = new Font("等线", 12, FontStyle.Bold))
            {
                SizeF sf = g.MeasureString(mark, f);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                StringFormat format = StringFormat.GenericTypographic;
                RectangleF rect = new RectangleF(5, 5, sf.Width, sf.Height);
                float dpi = g.DpiY;
                using (GraphicsPath gp = GetStringPath(mark, dpi, rect, f, format))
                {
                    g.DrawPath(p, gp); //描边
                    g.FillPath(b, gp); //填充
                }

                return bmp;
            }
        }
        private static GraphicsPath GetStringPath(string s, float dpi, RectangleF rect, Font font, StringFormat format)
        {
            GraphicsPath path = new GraphicsPath();
            // Convert font size into appropriate coordinates
            float emSize = dpi * font.SizeInPoints / 72;
            path.AddString(s, font.FontFamily, (int)font.Style, emSize, rect, format);

            return path;
        }

        private static void CreateContent()
        {
            if (!Directory.Exists(_imagePath))
                return;
            //var contentInfo = new FileInfo(_content);
            //contentInfo.Attributes = FileAttributes.Hidden;
            var info = new DirectoryInfo(_imagePath);
            Logger.Info("正在建立根目录的目录……");
            CreateDirContent(info);
            Logger.Info("正在建立子目录的文件目录……");
            foreach (var item in info.EnumerateDirectories())
            {
                //Logger.Debug(item.Name);
                CreateFileContent(item);
            }
        }

        private static void ClearContent()
        {
            if (!Directory.Exists(_imagePath))
                return;
            var info = new DirectoryInfo(_imagePath);
            RemoveContent(info);
            CreateContent();
        }

        private static void CreateDirContent(DirectoryInfo di)
        {
            string content = Path.Combine(di.FullName, ".content");
            List<string> list = di.EnumerateDirectories().Where(i => !i.Name.StartsWith('.')).Select(item => item.Name)
                .ToList();
            list.Sort();
            File.WriteAllLines(content, list);
        }

        private static void CreateFileContent(DirectoryInfo di)
        {
            string content = Path.Combine(di.FullName, ".content");
            List<string> list = di.EnumerateFiles().Where(i => !i.Name.StartsWith('.')).Select(item => item.Name)
                .ToList();
            list.Sort();
            File.WriteAllLines(content, list);
        }

        private static void RemoveContent(DirectoryInfo di)
        {
            string content = Path.Combine(di.FullName, ".content");
            File.Delete(content);
        }
    }
}
