using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;

namespace Daylily.Assist.Controllers
{
    public class ApiController : Controller
    {
        public static string CqRoot { get; set; } = "";

        public IActionResult ImgFile(string fileName)
        {
            try
            {
                string file = Path.Combine(CqRoot, "data", "image", fileName);
                if (System.IO.File.Exists(file))
                {
                    return Content(System.IO.File.ReadAllText(file));
                }
            }
            catch
            {
                // ignored
            }

            return NotFound();
        }

        public IActionResult StrokeString(string str)
        {
            Bitmap bmp = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(bmp);
            using (Brush b = new SolidBrush(Color.White))
            using (Pen p = new Pen(Color.FromArgb(96, 0, 0, 0), 2))
            using (Font f = new Font("等线", 12, FontStyle.Bold))
            {
                SizeF sf = g.MeasureString(str, f);
                g.Dispose();
                bmp = new Bitmap((int)sf.Width + 5, (int)sf.Height + 5);
                g = Graphics.FromImage(bmp);

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                StringFormat format = StringFormat.GenericTypographic;
                RectangleF rect = new RectangleF(5, 5, sf.Width, sf.Height);
                float dpi = g.DpiY;
                using (GraphicsPath gp = GetStringPath(str, dpi, rect, f, format))
                {
                    g.DrawPath(p, gp); //描边
                    g.FillPath(b, gp); //填充
                }
            }

            g.Dispose();
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            g.Dispose();
            bmp.Dispose();
            byte[] stream = ms.ToArray();
            return File(stream, "image/png");
        }

        private static GraphicsPath GetStringPath(string s, float dpi, RectangleF rect, Font font, StringFormat format)
        {
            GraphicsPath path = new GraphicsPath();
            // Convert font size into appropriate coordinates
            float emSize = dpi * font.SizeInPoints / 72;
            path.AddString(s, font.FontFamily, (int)font.Style, emSize, rect, format);

            return path;
        }

    }
}
