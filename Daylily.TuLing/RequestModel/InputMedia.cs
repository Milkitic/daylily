using Newtonsoft.Json;

namespace Daylily.TuLing.RequestModel
{
    /// <summary>
    /// 音频信息
    /// </summary>
    public class InputMedia
    {
        /// <summary>
        /// 音频地址
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}