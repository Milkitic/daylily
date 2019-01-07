using Newtonsoft.Json;

namespace Daylily.CoolQ.CoolQHttp.ResponseModel.Abstract
{
    public class StrangerInfo
    {
        [JsonProperty(PropertyName = "user_id")]
        public long UserId { get; set; }
        [JsonProperty(PropertyName = "nickname")]
        public string Nickname { get; set; }
        [JsonProperty(PropertyName = "sex")]
        public string Sex { get; set; }
        [JsonProperty(PropertyName = "age")]
        public string Age { get; set; }
    }
}
