using Newtonsoft.Json;

namespace Daylily.Common.Models.CQResponse
{
    public class GroupMsg : PrivateMsg
    {
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; set; }
        [JsonProperty(PropertyName = "anonymous")]
        public string Anonymous { get; set; }
        [JsonProperty(PropertyName = "anonymous_flag")]
        public string AnonymousFlag { get; set; }
    }
}
