using Newtonsoft.Json;

namespace Daylily.Common.Models.CQResponse.Api
{
    public class Base<T>
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        [JsonProperty(PropertyName = "retcode")]
        public int Retcode { get; set; }
        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }
    }
}
