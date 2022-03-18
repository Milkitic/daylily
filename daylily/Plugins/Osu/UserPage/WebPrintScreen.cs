using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Media;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace daylily.Plugins.Osu.UserPage;

/// <summary>
/// Windows Platform only!
/// </summary>
internal static class WebPrintScreen
{
    [DllImport("user32.dll")]
    // ReSharper disable once IdentifierTypo
    private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

    public static ImageSource? PrintBrowser(WebBrowser targetBrowser)
    {
        var screenWidth = (int)targetBrowser.ActualWidth;
        var screenHeight = (int)targetBrowser.ActualHeight;

        var browserHandle = targetBrowser.Handle;

        bool drawResult;
        using var bitmap = new Bitmap(screenWidth, screenHeight, PixelFormat.Format16bppRgb555);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            var graphicsHdc = graphics.GetHdc();
            try
            {
                drawResult = PrintWindow(browserHandle, graphicsHdc, 0);
            }
            finally
            {
                graphics.ReleaseHdc(graphicsHdc);
                graphics.Flush();
            }
        }

        if (!drawResult)
            return null;

        var imageSourceConverter = new ImageSourceConverter();
        var stream = new MemoryStream();
        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        return (ImageSource)imageSourceConverter.ConvertFrom(stream)!;
    }
}