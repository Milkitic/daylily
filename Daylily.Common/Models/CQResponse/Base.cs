using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Daylily.Common.Models.CQResponse
{
    public class Base
    {
        [JsonProperty(PropertyName = "post_type")]
        public string PostType { get; set; }
        [JsonProperty(PropertyName = "time")]
        public long Time { get; set; }
        [JsonProperty(PropertyName = "self_id")]
        public string SelfId { get; set; }
    }
}
