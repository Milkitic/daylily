using Newtonsoft.Json;

namespace Daylily.CoolQ.Models.CqResponse.Api
{
    public class DiscussMsgResponse
    {
        [JsonProperty(PropertyName = "message_id")]
        public long MessageId { get; set; }
    }
}
