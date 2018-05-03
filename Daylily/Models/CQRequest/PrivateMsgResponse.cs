using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.Models.CQRequest
{
    public class PrivateMsgResponse
    {
        [JsonProperty(PropertyName = "reply")]
        public string Reply { get; set; }
        [JsonProperty(PropertyName = "auto_escape")]
        public bool AutoEscape { get; set; } = false;
    }
}
