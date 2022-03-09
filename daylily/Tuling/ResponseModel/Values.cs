using System.Text.Json.Serialization;

namespace daylily.Tuling.ResponseModel
{
    public class Values
    {
        [JsonPropertyName("url")]
        public Uri? Url { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}