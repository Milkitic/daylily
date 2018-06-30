using Newtonsoft.Json;

namespace Daylily.Common.Models.CQResponse.Api
{
    public class GroupInfo
    {
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; set; }
        [JsonProperty(PropertyName = "group_name")]
        public string GroupName { get; set; }
    }
}
