using Newtonsoft.Json;

namespace Daylily.TuLing.RequestModel
{
    /// <summary>
    /// 图片信息
    /// </summary>
    public class InputImage
    {
        /// <summary>
        /// 图片地址
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}