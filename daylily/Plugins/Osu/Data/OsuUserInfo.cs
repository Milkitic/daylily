using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace daylily.Plugins.Osu.Data;

[Index(nameof(UserId), IsUnique = true)]
public class OsuUserInfo
{
    [Key]
    public int Id { get; set; }

    [JsonPropertyName("osuId")]
    public string UserId { get; set; } = null!;

    [JsonPropertyName("username")]
    public string Username { get; set; } = null!;

    [JsonIgnore]
    public string ModeId { get; set; } = null!;

    [JsonPropertyName("group")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Group Group { get; set; }

    [JsonPropertyName("level")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Level Level { get; set; }

    [JsonPropertyName("requestStatus")]
    public RequestStatus[]? RequestStatus { get; set; }

    [JsonPropertyName("requestLink")]
    public string? RequestLink { get; set; }

    [JsonIgnore]
    public string? UserPageText { get; set; }
}

public enum Group { Bn, Nat };

public enum Level { Full, Probation };

public enum RequestStatus { Closed, GameChat, GlobalQueue, PersonalQueue };