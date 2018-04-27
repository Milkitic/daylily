using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Models.CQRequest
{
    public class GroupMsgResponse
    {
        [JsonProperty(PropertyName = "reply")]
        public string Reply { get; set; }
        [JsonProperty(PropertyName = "auto_escape")]
        public bool AutoEscape { get; set; }
        [JsonProperty(PropertyName = "at_sender")]
        public bool AtSender { get; set; }
        [JsonProperty(PropertyName = "delete")]
        public bool Delete { get; set; }
        [JsonProperty(PropertyName = "kick")]
        public bool Kick { get; set; }
        [JsonProperty(PropertyName = "ban")]
        public bool Ban { get; set; }
    }
}
