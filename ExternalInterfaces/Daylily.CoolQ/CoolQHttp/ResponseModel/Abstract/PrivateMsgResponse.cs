using Newtonsoft.Json;

namespace Daylily.CoolQ.CoolQHttp.ResponseModel.Abstract
{
    public class PrivateMsgResponse
    {
        [JsonProperty(PropertyName = "message_id")]
        public long MessageId { get; set; }
    }
}
