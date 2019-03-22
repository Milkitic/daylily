using System;
using Newtonsoft.Json;

namespace Daylily.TuLing.ResponseModel
{
    public class Values
    {
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Url { get; set; }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
    }
}