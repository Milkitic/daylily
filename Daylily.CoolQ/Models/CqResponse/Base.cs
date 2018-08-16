using Newtonsoft.Json;

namespace Daylily.CoolQ.Models.CqResponse
{
    /// <summary>
    /// 上报消息中含有的字段。
    /// </summary>
    public abstract class Base
    {
        /// <summary>
        /// 上报类型，分别为message、notice、request。
        /// </summary>
        [JsonProperty(PropertyName = "post_type")]
        public string PostType { get; set; }
        /// <summary>
        /// 事件发生的时间戳。
        /// </summary>
        [JsonProperty(PropertyName = "time")]
        public long Time { get; set; }
        /// <summary>
        /// 收到消息的机器人 QQ 号。
        /// </summary>
        [JsonProperty(PropertyName = "self_id")]
        public string SelfId { get; set; }
    }
}
