using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
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
    [Version(0, 1, 2, PluginVersion.Beta)]
    [Help("查询一道深夜美食")]
    [Command("food")]
    public class Food : CommandApp
    {
        private static string _imagePath, _content;

        [FreeArg]
        public string FoodName { get; set; }

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


        }

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            string[] content = File.ReadAllLines(_content);
            string[] choices = FoodName == null
                ? content
                : content.Where(k => k.IndexOf(FoodName, StringComparison.Ordinal) != -1).ToArray();
            if (choices.Length == 0 && FoodName != null)
                return new CommonMessageResponse(
                    string.Format("没有找到 \"{0}\"", FoodName.Length > 30 ? FoodName.Substring(0, 27) + "..." : FoodName),
                    messageObj);

            string choice = choices[Rnd.Next(0, choices.Length)];
            string dir = Path.Combine(_imagePath, choice);

            string[] innerContent = File.ReadAllLines(Path.Combine(dir, ".content"));
            string innerChoice = innerContent[Rnd.Next(0, innerContent.Length)];
            string file = Path.Combine(dir, innerChoice);

            Bitmap bitmap = DrawWatermark(file);
            return new CommonMessageResponse(new FileImage(bitmap, 85).ToString(), messageObj);
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
                Logger.Debug(item.Name);
                CreateFileContent(item);
            }
        }

        private static void CreateDirContent(DirectoryInfo di)
        {
            string content = Path.Combine(di.FullName, ".content");
            List<string> list = di.EnumerateDirectories().Select(item => item.Name).ToList();
            list.Sort();
            File.WriteAllLines(content, list);
        }

        private static void CreateFileContent(DirectoryInfo di)
        {
            string content = Path.Combine(di.FullName, ".content");
            List<string> list = di.EnumerateFiles().Select(item => item.Name).ToList();
            list.Sort();
            File.WriteAllLines(content, list);
        }
    }
}
