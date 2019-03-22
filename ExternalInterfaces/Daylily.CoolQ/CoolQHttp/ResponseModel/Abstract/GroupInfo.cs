using Newtonsoft.Json;

namespace Daylily.CoolQ.CoolQHttp.ResponseModel.Abstract
{
    public class GroupInfo
    {
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; set; }
        [JsonProperty(PropertyName = "group_name")]
        public string GroupName { get; set; }
    }
}
