using System.ComponentModel.DataAnnotations;

namespace daylily.Plugins.Osu.Data;

public class BeatmapScan
{
    [Key]
    public int BeatmapSetId { get; set; }
    public List<BeatmapStat> BeatmapStats { get; set; }
    public List<BeatmapSubscribe> BeatmapSubscribes { get; set; }
}