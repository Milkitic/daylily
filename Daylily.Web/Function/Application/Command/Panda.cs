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
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Web.Function.Application.Command
{
    public class Panda : AppConstruct
    {
        public override string Name => "熊猫生成器";
        public override string Author => "yf_extension";
        public override PluginVersion Version => PluginVersion.Beta;
        public override string VersionNumber => "1.0";
        public override string Description => "生成熊猫图，表情随机";
        public override string Command => "panda";
        public override AppType AppType => AppType.Command;

        private static readonly string PandaDir = Path.Combine(Environment.CurrentDirectory, "panda");
        private static readonly string FontDir = Path.Combine(Environment.CurrentDirectory, "font");

        private readonly string[] _antiString = { "" };

        private const int MaxW = 250, MaxH = 220;
        private const FontStyle FontStyle = System.Drawing.FontStyle.Bold;
        private const int SmFontSize = 13, NmFontSize = 16, LgFontSize = 26;

        public override void OnLoad(string[] args)
        {
            throw new NotImplementedException();
        }

        public override CommonMessageResponse OnExecute(CommonMessage message)
        {
            if (message.MessageType == MessageType.Group && message.GroupId != "672076603")
                return new CommonMessageResponse(LoliReply.PrivateOnly, message);
            var resp = CqCode.EncodeImageToBase64(Draw(message.Parameter));
            return new CommonMessageResponse(resp, message);
        }

        private Bitmap Draw(string word)
        {

            FileInfo[] pandaArray = GetPandas().ToArray();
            FileInfo[] fontArray = GetFonts().ToArray();

            FileInfo fontInfo = fontArray[Rnd.Next(0, fontArray.Length)];
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile(fontInfo.FullName);

            FontFamily font = pfc.Families[0];
            FileInfo panda = pandaArray[Rnd.Next(0, pandaArray.Length)];

            string[] blankReply = { "傻逼，动动脑子写参数", "你倒是说话啊" };
            string[] invalidReply = { "你话太多了，沙雕" };

            if (word.Replace("\n", "").Replace("\r", "").Trim() == "")
                word = blankReply[Rnd.Next(0, blankReply.Length)];
            else if (!IsValid(word, panda, font))
                word = invalidReply[Rnd.Next(0, invalidReply.Length)];

            string[] lines = word.Split(',', '，');

            Bitmap bmp = new Bitmap(MaxW, MaxH);

            int renderSize = RenderSize(word);

            using (Image img = Image.FromFile(panda.FullName))
            using (Brush b = new SolidBrush(Color.Black))
            using (Font f = new Font(font, renderSize, FontStyle))
            {
                float maxWidth = 0, maxHeight = 0;
                List<float> eachWidth = new List<float>(), eachHeight = new List<float>();

                using (Graphics g1 = Graphics.FromImage(bmp))
                {
                    foreach (var item in lines)
                    {
                        SizeF sizeF = g1.MeasureString(item, f);
                        eachWidth.Add(sizeF.Width);
                        eachHeight.Add(sizeF.Height);
                        if (sizeF.Width > maxWidth) maxWidth = sizeF.Width;
                        maxHeight += sizeF.Height;
                    }
                }

                bmp = new Bitmap(maxWidth > img.Width + 10
                        ? (int)maxWidth
                        : img.Width + 10,
                    (int)maxHeight + img.Height + 4 > img.Height + 10
                        ? (int)maxHeight + img.Height + 10
                        : img.Height + 30,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(bmp);

                SetGraphicStyle(ref g, font);

                g.Clear(Color.White);

                float centerX = bmp.Width / 2f;
                //float centerY = bmp.Height / 2f;

                g.DrawImage(img, centerX - img.Width / 2f, 4, img.Width, img.Height);
                for (int i = 0; i < lines.Length; i++)
                {
                    g.DrawString(lines[i], f, b,
                        new PointF(centerX - eachWidth[i] / 2f, 7 + img.Height + i * eachHeight[i]));
                }

                g.Dispose();
            }

            return bmp;
        }

        private static int RenderSize(string word)
        {
            if (word.Length <= 5)
                return LgFontSize;
            if (word.Length <= 9)
                return NmFontSize;
            return SmFontSize;
        }

        private static IEnumerable<FileInfo> GetPandas() => new DirectoryInfo(PandaDir).GetFiles("d*.png");
        private static IEnumerable<FileInfo> GetFonts() => new DirectoryInfo(FontDir).GetFiles("*.tt?");

        private void SetGraphicStyle(ref Graphics g, FontFamily font)
        {
            g.TextRenderingHint = _antiString.Contains(font.Name)
                ? TextRenderingHint.ClearTypeGridFit
                : TextRenderingHint.AntiAlias;

            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
        }

        private static bool IsValid(string word, FileInfo panda, FontFamily font)
        {
            List<float> eachWidth = new List<float>(), eachHeight = new List<float>();
            string[] lines = word.Split(',', '，');
            int renderSize = RenderSize(word);

            Bitmap bmp = new Bitmap(MaxW, MaxH);
            using (Graphics g = Graphics.FromImage(bmp))
            using (Image img = Image.FromFile(panda.FullName))
            using (Font f = new Font(font, renderSize, FontStyle))
            {
                float maxWidth = 0, maxHeight = 0;
                foreach (var item in lines)
                {
                    SizeF sizeF = g.MeasureString(item, f);
                    eachWidth.Add(sizeF.Width);
                    eachHeight.Add(sizeF.Height);
                    if (sizeF.Width > maxWidth) maxWidth = sizeF.Width;
                    maxHeight += sizeF.Height;
                }

                if (maxWidth > MaxW || maxHeight > MaxH - img.Height)
                    return false;
            }

            return true;
        }
    }
}