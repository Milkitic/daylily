using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace daylily.Plugins.Osu.Data;

[Index(nameof(BeatmapSetId), IsUnique = true)]
public class BeatmapScan
{
    [Key]
    public int Id { get; set; }
    public int BeatmapSetId { get; set; }
    public List<BeatmapStat> BeatmapStats { get; set; }
    public List<BeatmapSubscribe> BeatmapSubscribes { get; set; }
}