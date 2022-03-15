using daylily.Plugins.Osu.Data;

namespace daylily.Plugins.Osu.BeatmapStats;

public class TickComparer : IEqualityComparer<BeatmapStat>
{
    private readonly TimeSpan _lidu;

    public TickComparer(TimeSpan lidu)
    {
        _lidu = lidu;
    }

    public bool Equals(BeatmapStat? x, BeatmapStat? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;

        return x.Timestamp.Ticks / _lidu.Ticks == y.Timestamp.Ticks / _lidu.Ticks;
    }

    public int GetHashCode(BeatmapStat obj)
    {
        return (obj.Timestamp.Ticks / _lidu.Ticks).GetHashCode();
    }
}