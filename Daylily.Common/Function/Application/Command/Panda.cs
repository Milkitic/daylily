using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Daylily.Common.Assist;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;
using Daylily.Common.Utils;
using Daylily.Common.Utils.LogUtils;

namespace Daylily.Common.Function.Application.Command
{
    [Name("熊猫生成器")]
    [Author("yf_extension")]
    [Version(0, 1, 3, PluginVersion.Beta)]
    [Help("生成可自定义文字的熊猫图（表情与文字随机）。")]
    [Command("panda")]
    public class Panda : CommandApp
    {
        [FreeArg]
        [Help("需要生成的配套文字。以逗号分隔作为行数。若带空格，请使用引号。")]
        public string PandaWord { get; set; }

        private static readonly string PandaDir = Path.Combine(Domain.CurrentDirectory, "panda");
        private static readonly string FontDir = Path.Combine(Domain.CurrentDirectory, "font");

        private readonly string[] _blankReply = { "傻B，动动脑子写参数", "你倒是说话啊" };
        private readonly string[] _invalidReply = { "你话太多了，沙雕" };

        private const int MaxW = 250, MaxH = 220;
        private const FontStyle FontStyle = System.Drawing.FontStyle.Regular;
        private const int SmFontSize = 13, NmFontSize = 16, LgFontSize = 24;

        private const string MagicalWord = "                              .";

        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            FontFamily font = GetRandFont(GetFonts());
            string pandaPath = GetRandPanda(GetPandas());

            string word = GetRealWord(font, pandaPath);
            string[] words = word.Split('\n');
            int renderSize = GetFontSize(word);

            var cqImg = new FileImage(Draw(words, renderSize, pandaPath, font), 65).ToString();
            return new CommonMessageResponse(cqImg, messageObj);
        }

        private static Bitmap Draw(IReadOnlyList<string> words, int renderSize, string pandaPath, FontFamily font)
        {
            words = words.Select(t => t.Replace("\r", "")).ToList();

            using (Image img = Image.FromFile(pandaPath))
            using (Brush b = new SolidBrush(Color.Black))
            using (Font f = new Font(font, renderSize, FontStyle))
            {
                float maxWidth = 0, maxHeight = 0;
                List<float> eachWidth = new List<float>(), eachHeight = new List<float>();
                GetRealSize(words, f, ref maxWidth, ref maxHeight, eachWidth, eachHeight);

                int padWidth = img.Width + 10;
                int padHeight = img.Height + 10;

                const int staticH = 20, padding = 4;

                int width = maxWidth > padWidth ? (int)maxWidth : padWidth,
                    height = (int)maxHeight + img.Height + padding > padHeight ? (int)maxHeight + padHeight : padHeight + staticH;
                Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    SetGraphicStyle(g);
                    g.Clear(Color.White);

                    float centerX = bmp.Width / 2f;

                    g.DrawImage(img, centerX - img.Width / 2f, 4, img.Width, img.Height);
                    for (int i = 0; i < words.Count; i++)
                    {
                        g.DrawString(words[i] + MagicalWord, f, b,
                            new PointF(centerX - eachWidth[i] / 2f, 5 + img.Height + i * eachHeight[i]));
                    }

                    g.Dispose();
                }

                return bmp;
            }
        }

        /// <summary>
        /// 确定文字是否超出容许的范围（防止生成的熊猫图过大）
        /// </summary>
        /// <returns></returns>
        private static bool IsLengthValid(string word, string pandaPath, FontFamily font)
        {
            string[] lines = word.Split(',', '，');
            int renderSize = GetFontSize(word);

            using (Bitmap bmp = new Bitmap(MaxW, MaxH))
            using (Graphics g = Graphics.FromImage(bmp))
            using (Image img = Image.FromFile(pandaPath))
            using (Font f = new Font(font, renderSize, FontStyle))
            {
                float maxWidth = 0, maxHeight = 0;
                foreach (var item in lines)
                {
                    SizeF sizeF = g.MeasureString(item, f);
                    if (sizeF.Width > maxWidth) maxWidth = sizeF.Width;
                    maxHeight += sizeF.Height;

                    if (maxWidth > MaxW || maxHeight > MaxH - img.Height)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 确定画布实际大小。
        /// </summary>
        private static void GetRealSize(IEnumerable<string> words, Font f, ref float maxWidth, ref float maxHeight,
            ICollection<float> eachWidth, ICollection<float> eachHeight)
        {
            using (Bitmap bmp = new Bitmap(MaxW, MaxH))
            using (Graphics g1 = Graphics.FromImage(bmp))
            {
                foreach (var item in words)
                {
                    SizeF sizeF = g1.MeasureString(item + MagicalWord, f);
                    SizeF sizeF2 = g1.MeasureString(MagicalWord, f);
                    float width = sizeF.Width - sizeF2.Width;
                    eachWidth.Add(width);
                    eachHeight.Add(sizeF.Height);
                    if (width > maxWidth) maxWidth = width;
                    maxHeight += sizeF.Height;
                }
            }
        }

        /// <summary>
        /// 确定字体是否为空或有效。若为空或无效，则用默认语句替代。
        /// </summary>
        /// <returns></returns>
        private string GetRealWord(FontFamily font, string pandaPath)
        {
            string word = CqCode.Decode(PandaWord.Replace("！", "!").Replace("？", "?"));
            if (word == null || word.Replace("\n", "").Replace("\r", "").Trim() == "")
                word = _blankReply[Rnd.Next(0, _blankReply.Length)];
            else if (!IsLengthValid(word, pandaPath, font))
                word = _invalidReply[Rnd.Next(0, _invalidReply.Length)];
            return word;
        }

        /// <summary>
        /// 确定字体大小，由字体数量决定。
        /// </summary>
        /// <returns></returns>
        private static int GetFontSize(string word)
        {
            if (word.Length <= 5)
                return LgFontSize;
            if (word.Length <= 9)
                return NmFontSize;
            return SmFontSize;
        }

        /// <summary>
        /// 获取目录下的熊猫。
        /// </summary>
        /// <returns></returns>
        private static FileInfo[] GetPandas() => new DirectoryInfo(PandaDir).GetFiles("d*.png");

        /// <summary>
        /// 获取目录下的字体。
        /// </summary>
        /// <returns></returns>
        private static FileInfo[] GetFonts() => new DirectoryInfo(FontDir).GetFiles("*.tt?");

        /// <summary>
        /// 随机获取一个的熊猫。
        /// </summary>
        /// <returns></returns>
        private static string GetRandPanda(IReadOnlyList<FileInfo> pandaArray)
        {
            return pandaArray[Rnd.Next(0, pandaArray.Count)].FullName;
        }

        /// <summary>
        /// 随机获取一个的字体。
        /// </summary>
        /// <returns></returns>
        private static FontFamily GetRandFont(IReadOnlyList<FileInfo> fontArray)
        {
            FileInfo fontInfo = fontArray[Rnd.Next(0, fontArray.Count)];
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile(fontInfo.FullName);
            FontFamily font = pfc.Families[0];
            return font;
        }

        private static void SetGraphicStyle(Graphics g)
        {
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
        }
    }
}