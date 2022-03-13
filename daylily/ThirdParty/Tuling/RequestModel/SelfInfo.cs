using System.Text.Json.Serialization;

namespace daylily.ThirdParty.Tuling.RequestModel
{
    /// <summary>
    /// 客户端属性
    /// </summary>
    public class SelfInfo
    {
        /// <summary>
        /// 地理位置信息
        /// </summary>
        [JsonPropertyName("location")]
        public Location Location { get; set; }
    }
}