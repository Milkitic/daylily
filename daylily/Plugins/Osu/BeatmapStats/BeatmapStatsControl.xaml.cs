using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Microsoft.Extensions.Logging;
using MilkiBotFramework;
using MilkiBotFramework.Connecting;
using MilkiBotFramework.Imaging.Wpf;
using MilkiBotFramework.Plugining.Configuration;
using Image = SixLabors.ImageSharp.Image;

namespace daylily.Plugins.Osu.BeatmapStats;

/// <summary>
/// BeatmapStatsControl.xaml 的交互逻辑
/// </summary>
public partial class BeatmapStatsControl : WpfDrawingControl
{
    private readonly ILogger _logger;
    private readonly BotOptions _botOptions;
    private readonly LightHttpClient _lightHttpClient;
    private readonly BeatmapStatsVm _viewModel;

    public BeatmapStatsControl(
        ILogger logger,
        BotOptions botOptions,
        LightHttpClient lightHttpClient,
        object viewModel,
        Image? sourceImage = null)
        : base(viewModel, sourceImage)
    {
        _logger = logger;
        _botOptions = botOptions;
        _lightHttpClient = lightHttpClient;
        _viewModel = (BeatmapStatsVm)ViewModel;
        Initialized += async (_, _) =>
        {
            await ResetData();
        };
        InitializeComponent();
    }

    private async Task ResetData()
    {
        var firstOrDefault = _viewModel.Stats;
        var startTime = firstOrDefault.Min(k => k.Timestamp);
        var endTime = firstOrDefault.Max(k => k.Timestamp);
        //var endTime = startTime.AddDays(1);
        var i = 90;
        var lidu = (endTime - startTime) / (i * 3f);
        //var lidu = TimeSpan.FromDays(1);
        var dayConfig = Mappers.Xy<DateModel<int>>()
            .X(dateModel => dateModel.Timestamp.Ticks / lidu.Ticks)
            .Y(dateModel => dateModel.Value);

        var series = new SeriesCollection(dayConfig)
        {
            new LineSeries
            {
                Title = "Favorite Count",
                Values = new ChartValues<DateModel<int>>(firstOrDefault
                    .Where(k => k.Timestamp >= startTime && k.Timestamp <= endTime)
                    .Distinct(new TickComparer(lidu))
                    .Distinct(new FavoriteComparer())
                    .Select(k => new DateModel<int>()
                    {
                        Timestamp = k.Timestamp,
                        Value = (int) k.FavoriteCount
                    })),
                Fill = Brushes.Transparent,
                PointGeometrySize = 6,
                ScalesYAt = 0,
                Stroke = Brushes.ForestGreen
            },
            new LineSeries
            {
                Title = "Play Count",
                Values = new ChartValues<DateModel<int>>(firstOrDefault
                    .Where(k => k.Timestamp >= startTime && k.Timestamp <= endTime)
                    .Distinct(new TickComparer(lidu))
                    .Distinct(new PlayCountComparer())
                    .Select(k => new DateModel<int>()
                    {
                        Timestamp = k.Timestamp,
                        Value = (int) k.PlayCount
                    })),

                Fill = Brushes.Transparent,
                ScalesYAt = 1,
                PointGeometrySize = 0,
                Stroke = Brushes.CornflowerBlue
            },
        };

        _viewModel.AxisYCollection = new AxesCollection
        {
            new Axis
            {
                Title = "Favorite Count", Foreground = Brushes.ForestGreen, FontSize = 15,
                LabelFormatter = NumberLabelFormatter
            },
            new Axis
            {
                Title = "Play Count", Foreground = Brushes.CornflowerBlue, FontSize = 15,
                LabelFormatter = NumberLabelFormatter
            }
        };
        _viewModel.AxisYCollection[0].Separator.StrokeThickness = 0;
        //_viewModel.AxisYCollection[1].Separator.StrokeThickness = 0;

        _viewModel.Formatter = value => new DateTime((long)(value * lidu.Ticks)).ToString("yyyy-M-d H:mm");
        _viewModel.Series = series;

        _viewModel.SubmittedDateLocal = _viewModel.Beatmapset?.SubmittedDate.ToLocalTime();
        _viewModel.LastUpdatedLocal = _viewModel.Beatmapset?.LastUpdated.ToLocalTime();
        _viewModel.RankedDateLocal = _viewModel.Beatmapset?.RankedDate?.ToLocalTime();
        var link = _viewModel.Beatmapset?.Covers.List2X;

        try
        {
            var guid = Path.GetRandomFileName();
            if (!Directory.Exists(_botOptions.CacheImageDir))
                Directory.CreateDirectory(_botOptions.CacheImageDir);
            var path = await _lightHttpClient.SaveImageFromUrlAsync(link, _botOptions.CacheImageDir, guid);

            var bi = new BitmapImage();

            // Begin initialization.
            bi.BeginInit();

            // Set properties.
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(path);

            // End initialization.
            bi.EndInit();
            bi.Freeze(); //Important to freeze it, otherwise it will still have minor leaks

            _viewModel.List2XSource = bi;
            await Task.Delay(1);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("下载osu!图片时出错：" + ex.Message);
        }

        await FinishDrawing();
    }
    private string NumberLabelFormatter(double value)
    {
        return Convert.ToInt32(value).ToString("N0");
    }
}