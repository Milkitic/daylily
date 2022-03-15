using System.ComponentModel.DataAnnotations;

namespace daylily.Plugins.Osu.Data;

public class BeatmapSubscribe
{
    [Key]
    public int Id { get; set; }
    public long QQ { get; set; }
    public BeatmapScan BeatmapScan { get; set; }
}