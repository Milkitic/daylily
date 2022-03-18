using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace daylily.Plugins.Osu.Data;

public class BeatmapStat
{
    [Key]
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public long FavoriteCount { get; set; }
    public long PlayCount { get; set; }

    [JsonIgnore]
    public BeatmapScan BeatmapScan { get; set; }
}