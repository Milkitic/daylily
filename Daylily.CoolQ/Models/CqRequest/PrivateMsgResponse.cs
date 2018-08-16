using Newtonsoft.Json;

namespace Daylily.CoolQ.Models.CqRequest
{
    public class PrivateMsgResponse
    {
        [JsonProperty(PropertyName = "reply")]
        public string Reply { get; set; }
        [JsonProperty(PropertyName = "auto_escape")]
        public bool AutoEscape { get; set; }
    }
}
