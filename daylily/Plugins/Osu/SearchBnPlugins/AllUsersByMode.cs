using System.Text.Json.Serialization;
using daylily.Plugins.Osu.Data;

namespace daylily.Plugins.Osu.SearchBnPlugins;

public class AllUsersByMode
{
    [JsonPropertyName("_id")] 
    public string ModeId { get; set; } = null!;

    [JsonPropertyName("users")]
    public OsuUserInfo[] Users { get; set; } = Array.Empty<OsuUserInfo>();
}