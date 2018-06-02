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

namespace Daylily.Web.Function.Application.Command
{
    public class Panda : AppConstruct
    {
        private static readonly string PandaDir = Path.Combine(Environment.CurrentDirectory, "panda");
        private static readonly string FontDir = Path.Combine(Environment.CurrentDirectory, "font");

        public override CommonMessageResponse Execute(CommonMessage message)
        {
            if (message.MessageType == MessageType.Group && message.GroupId != "672076603")
                return new CommonMessageResponse("请私聊…", message);
            var resp = CqCode.EncodeImageToBase64(Draw(message.Parameter));
            //Logger.InfoLine(resp);
            return new CommonMessageResponse(resp, message);
        }

        private static Bitmap Draw(string word)
        {
            const int maxW = 250, maxH = 220;
            Random rnd = new Random();
            string[] blankReply = { "傻逼，动动脑子写参数", "你倒是说话啊" };
            if (word.Trim() == "") word = blankReply[rnd.Next(0, blankReply.Length)];
            string[] lines = word.Split(',', '，');
            Bitmap bmp = new Bitmap(maxW, maxH);

            DirectoryInfo di = new DirectoryInfo(FontDir);
            var ok = di.GetFiles("*.tt?");
            FileInfo rndFont = ok[rnd.Next(0, ok.Length)];
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile(rndFont.FullName);

            using (Image img = Image.FromFile(Path.Combine(PandaDir, $"d{rnd.Next(0, 5)}.png")))
            //using (Image img = Image.FromFile(Path.Combine(PandaDir, "d2.png")))
            using (Brush b = new SolidBrush(Color.Black))
            using (Font f = word.Length <= 5 ? new Font(pfc.Families[0], 26, FontStyle.Bold) : new Font(pfc.Families[0], 13, FontStyle.Bold))
            {
                float maxWidth = 0, maxHeight = 0;
                List<float> eachWidth = new List<float>(), eachHeight = new List<float>();
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    foreach (var item in lines)
                    {
                        SizeF sizeF = g.MeasureString(item, f);
                        eachWidth.Add(sizeF.Width);
                        eachHeight.Add(sizeF.Height);
                        if (sizeF.Width > maxWidth) maxWidth = sizeF.Width;
                        maxHeight += sizeF.Height;
                    }

                    if (maxWidth > maxW || maxHeight > maxH - img.Height)
                    {
                        maxWidth = 0;
                        maxHeight = 0;
                        lines = new[] { "你字写的太多了", "沙雕" };
                        eachWidth.Clear();
                        eachHeight.Clear();
                        foreach (var item in lines)
                        {
                            SizeF size = g.MeasureString(item, f);
                            eachWidth.Add(size.Width);
                            eachHeight.Add(size.Height);
                            if (size.Width > maxWidth) maxWidth = size.Width;
                            maxHeight += size.Height;
                        }
                    }
                }


                bmp = new Bitmap(maxWidth > img.Width + 50 ? (int)maxWidth : img.Width + 50, (int)maxHeight + img.Height + 4 > img.Height + 50 ? (int)maxHeight + img.Height + 10 : img.Height + 50,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    if (rndFont.Name.ToUpper() == "STZHONGS.TTF" && 1 == 1)
                    {
                        g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    }
                    else
                    {
                        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    }
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.Clear(Color.White);

                    float centerX = bmp.Width / 2f, centerY = bmp.Height / 2f;

                    g.DrawImage(img, centerX - img.Width / 2f, 4, img.Width, img.Height);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        g.DrawString(lines[i], f, b, new PointF(centerX - eachWidth[i] / 2f, 7 + img.Height + i * eachHeight[0]));
                    }
                }

            }
            return bmp;
        }
    }
}
