using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace daylily.Plugins.Osu.Data;

[Index(nameof(ScribeUserId), nameof(BeatmapScanId), IsUnique = true)]
public class BeatmapSubscribe
{
    [Key]
    public int Id { get; set; }
    public string ScribeUserId { get; set; }
    public BeatmapScan BeatmapScan { get; set; }
    public int BeatmapScanId { get; set; }
}