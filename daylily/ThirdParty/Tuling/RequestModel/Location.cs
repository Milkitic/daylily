using System.Text.Json.Serialization;

namespace daylily.Tuling.RequestModel
{
    /// <summary>
    /// 地理位置信息
    /// </summary>
    public class Location
    {
        /// <summary>
        /// 所在城市
        /// </summary>
        [JsonPropertyName("city")]
        public string City { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        [JsonPropertyName("province")]
        public string Province { get; set; }

        /// <summary>
        /// 街道
        /// </summary>
        [JsonPropertyName("street")]
        public string Street { get; set; }
    }
}