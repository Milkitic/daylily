using Newtonsoft.Json;

namespace Daylily.Common.Models.CQRequest
{
    public class PrivateMsgResponse
    {
        [JsonProperty(PropertyName = "reply")]
        public string Reply { get; set; }
        [JsonProperty(PropertyName = "auto_escape")]
        public bool AutoEscape { get; set; }
    }
}
