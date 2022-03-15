using daylily.Plugins.Osu.Data;

namespace daylily.Plugins.Osu.BeatmapStats;

public class PlayCountComparer : IEqualityComparer<BeatmapStat>
{
    public bool Equals(BeatmapStat? x, BeatmapStat? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.PlayCount == y.PlayCount;
    }

    public int GetHashCode(BeatmapStat obj)
    {
        return obj.PlayCount.GetHashCode();
    }
}