using System.Windows.Media;
using Coosu.Api.V2.ResponseModels;
using daylily.Plugins.Osu.Data;
using LiveCharts;
using LiveCharts.Wpf;
using MilkiBotFramework.Data;

namespace daylily.Plugins.Osu.BeatmapStats;

internal class BeatmapStatsVm : ViewModelBase
{
    private Func<double, string> _formatter;
    private SeriesCollection _series;
    private AxesCollection _axisYCollection;
    private ImageSource _list2XSource;
    private DateTimeOffset? _submittedDateLocal;
    private DateTimeOffset? _lastUpdatedLocal;
    private DateTimeOffset? _rankedDateLocal;

    public Func<double, string> Formatter
    {
        get => _formatter;
        set => this.RaiseAndSetIfChanged(ref _formatter, value);
    }

    public SeriesCollection Series
    {
        get => _series;
        set => this.RaiseAndSetIfChanged(ref _series, value);
    }

    public AxesCollection AxisYCollection
    {
        get => _axisYCollection;
        set => this.RaiseAndSetIfChanged(ref _axisYCollection, value);
    }

    public ImageSource List2XSource
    {
        get => _list2XSource;
        set => this.RaiseAndSetIfChanged(ref _list2XSource, value);
    }

    public DateTimeOffset? SubmittedDateLocal
    {
        get => _submittedDateLocal;
        set => this.RaiseAndSetIfChanged(ref _submittedDateLocal, value);
    }

    public DateTimeOffset? LastUpdatedLocal
    {
        get => _lastUpdatedLocal;
        set => this.RaiseAndSetIfChanged(ref _lastUpdatedLocal, value);
    }

    public DateTimeOffset? RankedDateLocal
    {
        get => _rankedDateLocal;
        set => this.RaiseAndSetIfChanged(ref _rankedDateLocal, value);
    }

    public Beatmapset? Beatmapset { get; init; }
    public List<BeatmapStat> Stats { get; init; } = null!;
}