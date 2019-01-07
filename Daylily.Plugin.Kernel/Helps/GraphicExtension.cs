using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Daylily.Plugin.Kernel.Helps
{
    public static class GraphicExtension
    {
        public static void DrawRoundRectangle(this Graphics g, Pen pen, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.DrawPath(pen, path);
            }
        }

        public static void FillRoundRectangle(this Graphics g, Brush brush, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.FillPath(brush, path);
            }
        }

        public static void InitialHighQuality(this Graphics g)
        {
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
        }

        public static Size MeasureStringSize(this Graphics g, string str, Font font)
        {
            var ok = g.MeasureString(str, font);
            return new Size((int)ok.Width, (int)ok.Height);
        }

        public static void FillImage(this Graphics g, Image backgroundImage, Size imageSize)
        {
            RectangleF cutRectangle = GetBgSize(imageSize, backgroundImage.Size);
            g.DrawImage(backgroundImage, cutRectangle);
        }

        public static RectangleF GetBgSize(Size canvasSize, Size bgSize)
        {
            const int canvasX = 0, canvasY = 0;
            float canvasWidth = canvasSize.Width, canvasHeight = canvasSize.Height;
            float ratio = (float)canvasSize.Width / canvasSize.Height;
            float bgRatio = (float)bgSize.Width / bgSize.Height;

            if (bgRatio >= ratio) // more width
            {
                float scale = canvasHeight / bgSize.Height;
                float height = canvasHeight;
                float width = bgSize.Width * scale;
                float x = -(width - canvasWidth) / 2;
                float y = canvasY;
                return new RectangleF(x, y, width, height);
            }
            else // more height
            {
                float scale = canvasWidth / bgSize.Width;
                float width = canvasWidth;
                float height = bgSize.Height * scale;
                float x = canvasX;
                float y = -(height - canvasHeight) / 2;
                return new RectangleF(x, y, width, height);
            }
        }

        private static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }
    }
}
