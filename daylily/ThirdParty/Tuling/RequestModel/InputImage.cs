using System.Text.Json.Serialization;

namespace daylily.ThirdParty.Tuling.RequestModel
{
    /// <summary>
    /// 图片信息
    /// </summary>
    public class InputImage
    {
        /// <summary>
        /// 图片地址
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}