using Newtonsoft.Json;

namespace Daylily.TuLing.RequestModel
{
    /// <summary>
    /// 客户端属性
    /// </summary>
    public class SelfInfo
    {
        /// <summary>
        /// 地理位置信息
        /// </summary>
        [JsonProperty("location")]
        public Location Location { get; set; }
    }
}