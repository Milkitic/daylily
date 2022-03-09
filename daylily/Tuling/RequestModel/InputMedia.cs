using System.Text.Json.Serialization;

namespace daylily.Tuling.RequestModel
{
    /// <summary>
    /// 音频信息
    /// </summary>
    public class InputMedia
    {
        /// <summary>
        /// 音频地址
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}