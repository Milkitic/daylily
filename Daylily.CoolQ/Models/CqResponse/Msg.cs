using Newtonsoft.Json;

namespace Daylily.CoolQ.Models.CqResponse
{
    /// <summary>
    /// 私聊消息。
    /// </summary>
    public abstract class Msg : Base
    {
        /// <summary>
        /// 消息类型。
        /// </summary>
        [JsonProperty(PropertyName = "message_type")]
        public string MessageType { get; set; }
        /// <summary>
        /// 消息子类型，如果是好友则是 friend，如果从群或讨论组来的临时会话则分别是 group、discuss。
        /// </summary>
        [JsonProperty(PropertyName = "sub_type")]
        public string SubType { get; set; }
        /// <summary>
        /// 消息 ID。
        /// </summary>
        [JsonProperty(PropertyName = "message_id")]
        public long MessageId { get; set; }
        /// <summary>
        /// 发送者 QQ 号。
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public long UserId { get; set; }
        /// <summary>
        /// 消息内容。
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
        /// <summary>
        /// 原始消息内容。
        /// </summary>
        [JsonProperty(PropertyName = "raw_message")]
        public string RawMessage { get; set; }
        /// <summary>
        /// 字体。
        /// </summary>
        [JsonProperty(PropertyName = "font")]
        public long Font { get; set; }
    }
}
