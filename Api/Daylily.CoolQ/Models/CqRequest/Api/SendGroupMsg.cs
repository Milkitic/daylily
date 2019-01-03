using Newtonsoft.Json;

namespace Daylily.CoolQ.Models.CqRequest.Api
{
    public class SendGroupMsg
    {
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
        [JsonProperty(PropertyName = "auto_escape")]
        public bool AutoEscape { get; set; }

        public SendGroupMsg(string groupId, string message, bool autoEscape = false)
        {
            GroupId = long.Parse(groupId);
            Message = message;
            AutoEscape = autoEscape;
        }
    }
}
