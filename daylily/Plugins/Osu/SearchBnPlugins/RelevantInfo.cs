using System.Text.Json.Serialization;

namespace daylily.Plugins.Osu.SearchBnPlugins;

public class RelevantInfo
{
    [JsonPropertyName("allUsersByMode")]
    public AllUsersByMode[] AllUsersByMode { get; set; } = Array.Empty<AllUsersByMode>();
}