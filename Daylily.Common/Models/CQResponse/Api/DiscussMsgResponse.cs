using Newtonsoft.Json;

namespace Daylily.Common.Models.CQResponse.Api
{
    public class DiscussMsgResponse
    {
        [JsonProperty(PropertyName = "message_id")]
        public long MessageId { get; set; }
    }
}
