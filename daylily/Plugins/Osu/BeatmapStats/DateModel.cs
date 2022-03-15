namespace daylily.Plugins.Osu.BeatmapStats;

public class DateModel<T>
{
    public DateTime Timestamp { get; set; }
    public T Value { get; set; }
}