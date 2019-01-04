using Newtonsoft.Json;

namespace Daylily.CoolQ.Models.CqResponse
{
    /// <summary>
    /// 群消息。
    /// </summary>
    public class CoolQGroupMessageApi : CoolQMessageApi
    {
        /// <summary>
        /// 群号。
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; set; }

        /// <summary>
        /// 匿名信息，如果不是匿名消息则为 null。
        /// </summary>
        [JsonProperty(PropertyName = "anonymous")]
        public Anonymous Anonymous { get; set; }

    }

    /// <summary>
    /// 匿名信息，如果不是匿名消息则为 null。
    /// </summary>
    public class Anonymous
    {
        /// <summary>
        /// 匿名用户 ID。
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        /// <summary>
        /// 匿名用户名称。
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 匿名用户 flag，在调用禁言 API 时需要传入。
        /// </summary>
        [JsonProperty(PropertyName = "flag")]
        public string Flag { get; set; }
    }
}
