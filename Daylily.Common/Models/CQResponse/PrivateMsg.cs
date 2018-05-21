using Newtonsoft.Json;

namespace Daylily.Common.Models.CQResponse
{
    public class PrivateMsg
    {
        [JsonProperty(PropertyName = "time")]
        public long Time { get; set; }
        [JsonProperty(PropertyName = "post_type")]
        public string PostType { get; set; }
        [JsonProperty(PropertyName = "message_type")]
        public string MessageType { get; set; }
        [JsonProperty(PropertyName = "sub_type")]
        public string SubType { get; set; }
        [JsonProperty(PropertyName = "message_id")]
        public long MessageId { get; set; }
        [JsonProperty(PropertyName = "user_id")]
        public long UserId { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
        [JsonProperty(PropertyName = "font")]
        public long Font { get; set; }
        [JsonProperty(PropertyName = "self_id")]
        public string SelfId { get; set; }
    }
}
