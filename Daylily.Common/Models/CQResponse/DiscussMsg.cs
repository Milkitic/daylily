using Newtonsoft.Json;

namespace Daylily.Common.Models.CQResponse
{
    public class DiscussMsg : PrivateMsg
    {
        [JsonProperty(PropertyName = "discuss_id")]
        public long DiscussId { get; set; }
    }
}
