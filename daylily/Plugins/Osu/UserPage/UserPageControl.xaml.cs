using System.IO;
using System.Text.Json;
using System.Web;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MilkiBotFramework.Imaging.Wpf;
using mshtml;
using Image = SixLabors.ImageSharp.Image;

namespace daylily.Plugins.Osu.UserPage
{
    /// <summary>
    /// UserPageControl.xaml 的交互逻辑
    /// </summary>
    public partial class UserPageControl : WpfDrawingControl
    {
        private static string? _templateText;

        private readonly string _pluginHome;
        private readonly UserPageVm _viewModel;
        private readonly DispatcherTimer _dispatcherTimer;

        public UserPageControl(string pluginHome, object viewModel, Image? sourceImage = null)
            : base(viewModel, sourceImage)
        {
            _pluginHome = pluginHome;
            _viewModel = (UserPageVm)viewModel;
            _dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };

            InitializeComponent();
        }

        public override async Task<MemoryStream> ProcessOnceAsync()
        {
            var source = WebPrintScreen.PrintBrowser(WebBrowser);
            if (source == null)
                throw new Exception("WebBrowser print failed!");
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)source));
            var stream = new MemoryStream();
            encoder.Save(stream);
            stream.Position = 0;
            return stream;
        }

        private void UserPageControl_OnInitialized(object? sender, EventArgs e)
        {
            _templateText ??= File.ReadAllText(Path.Combine(_pluginHome, "bbcode", "template.txt"));
            _dispatcherTimer.Start();
            _dispatcherTimer.Tick += DispatcherTimerTick;

            var html = _templateText.Replace("{{content}}", _viewModel.RawHtml);
            var htmlPath = Path.Combine(_pluginHome, $"u{_viewModel.UserId}.html");
            File.WriteAllText(htmlPath, html);

            var fullPath = Path.GetFullPath(htmlPath);
            var htmlPathUri = HttpUtility.UrlEncode(fullPath.Replace("\\", "/"));
            WebBrowser.Navigate(new Uri(@"file:///" + htmlPathUri));
        }

        private async void DispatcherTimerTick(object? sender, EventArgs e)
        {
            dynamic doc = WebBrowser.Document;
            if (doc is not IHTMLDocument2 thisDoc)
                return;

            var element = thisDoc.all
                .OfType<IHTMLElement>()
                .FirstOrDefault(k => k?.id == "hidden");
            var html = element?.innerText;
            if (html == null) return;

            var jDoc = JsonDocument.Parse(html);
            var width = jDoc.RootElement.GetProperty("width").GetInt32();
            var height = jDoc.RootElement.GetProperty("height").GetInt32();
            WebBrowser.Width = width + 9;
            WebBrowser.Height = height + 2;
            _dispatcherTimer.Stop();
            await Task.Delay(100);
            await FinishDrawing();
        }
    }
}
