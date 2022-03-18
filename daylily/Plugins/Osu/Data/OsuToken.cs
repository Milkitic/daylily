using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace daylily.Plugins.Osu.Data;

[Table("tokens")]
public class OsuToken
{
    [Key]
    public string SourceId { get; set; }
    public string TokenType { get; set; }
    public long ExpiresIn { get; set; }
    public string AccessToken { get; set; }
    public DateTimeOffset? CreateTime { get; set; }
    public string RefreshToken { get; set; }
    public bool IsPublic { get; set; }
    public long? UserId { get; set; }
}