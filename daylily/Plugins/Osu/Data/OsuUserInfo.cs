using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace daylily.Plugins.Osu.Data;

public class OsuUserInfo
{
    [Key]
    [JsonPropertyName("osuId")]
    public int Id { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = null!;

    [JsonIgnore]
    public HashSet<ModeId> ModeIds { get; set; } = new();

    [JsonPropertyName("group")]
    public Group Group { get; set; }

    [JsonPropertyName("level")]
    public Level Level { get; set; }

    [JsonPropertyName("requestStatus")]
    public RequestStatus[]? RequestStatus { get; set; }

    [JsonPropertyName("requestLink")]
    public string? RequestLink { get; set; }

    [JsonIgnore]
    public string? UserPageText { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Group { Bn, Nat };

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Level { Full, Probation };

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RequestStatus { Closed, GameChat, GlobalQueue, PersonalQueue };

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ModeId { Osu, Catch, Mania, Taiko };