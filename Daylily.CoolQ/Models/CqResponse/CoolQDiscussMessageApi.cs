using Newtonsoft.Json;

namespace Daylily.CoolQ.Models.CqResponse
{
    /// <summary>
    /// 讨论组消息。
    /// </summary>
    public class CoolQDiscussMessageApi : CoolQMessageApi
    {
        /// <summary>
        /// 讨论组 ID。
        /// </summary>
        [JsonProperty(PropertyName = "discuss_id")]
        public long DiscussId { get; set; }
    }
}
