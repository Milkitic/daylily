using Newtonsoft.Json;

namespace Daylily.CoolQ.CoolQHttp.ResponseModel.Abstract
{
    public class GroupMsgResponse
    {
        [JsonProperty(PropertyName = "message_id")]
        public long MessageId { get; set; }
    }
}
