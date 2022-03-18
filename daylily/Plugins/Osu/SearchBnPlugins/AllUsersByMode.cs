using System.Text.Json.Serialization;
using daylily.Plugins.Osu.Data;

namespace daylily.Plugins.Osu.SearchBnPlugins;

public class AllUsersByMode
{
    [JsonPropertyName("_id")]
    public ModeId ModeId { get; set; }

    [JsonPropertyName("users")]
    public OsuUserInfo[] Users { get; set; } = Array.Empty<OsuUserInfo>();
}