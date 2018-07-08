using Newtonsoft.Json;

namespace Daylily.Common.Models.CQResponse
{
    /// <summary>
    /// 讨论组消息。
    /// </summary>
    public class DiscussMsg : Msg
    {
        /// <summary>
        /// 讨论组 ID。
        /// </summary>
        [JsonProperty(PropertyName = "discuss_id")]
        public long DiscussId { get; set; }
    }
}
