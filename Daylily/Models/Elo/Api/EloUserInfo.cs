using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.Models.Elo.Api
{
    public class EloUserInfo
    {
        [JsonProperty(PropertyName = "result")]
        public string Result { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
        [JsonProperty(PropertyName = "user")]
        public EloUser User { get; set; }
    }
    public class EloUser
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }
        [JsonProperty(PropertyName = "user_id")]
        public long UserId { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "elo")]
        public double Elo { get; set; }
        [JsonProperty(PropertyName = "ranking")]
        public long Ranking { get; set; }
    }
}
