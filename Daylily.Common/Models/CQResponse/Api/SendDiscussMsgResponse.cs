using Newtonsoft.Json;

namespace Daylily.Common.Models.CQResponse.Api
{
    public class SendDiscussMsgResponse
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        [JsonProperty(PropertyName = "retcode")]
        public int Retcode { get; set; }
        [JsonProperty(PropertyName = "data")]
        public _SendDiscussMsgResponse Data { get; set; }
    }
    public class _SendDiscussMsgResponse
    {
        [JsonProperty(PropertyName = "message_id")]
        public long MessageId { get; set; }
    }
}
