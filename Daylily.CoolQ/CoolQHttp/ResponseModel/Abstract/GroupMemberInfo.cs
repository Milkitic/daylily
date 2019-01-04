using System;
using Newtonsoft.Json;

namespace Daylily.CoolQ.CoolQHttp.ResponseModel.Abstract
{
    public class GroupMember
    {
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; set; }
        [JsonProperty(PropertyName = "user_id")]
        public long UserId { get; set; }
        [JsonProperty(PropertyName = "nickname")]
        public string Nickname { get; set; }
        [JsonProperty(PropertyName = "card")]
        public string Card { get; set; }
        [JsonProperty(PropertyName = "sex")]
        public string Sex { get; set; }
        [JsonProperty(PropertyName = "age")]
        public int Age { get; set; }
        [JsonProperty(PropertyName = "area")]
        public string Area { get; set; }
        [JsonProperty(PropertyName = "join_time")]
        public long TsJoinTime { get; set; }
        [JsonProperty(PropertyName = "last_sent_time")]
        public long TsLastSentTime { get; set; }
        [JsonProperty(PropertyName = "level")]
        public string Level { get; set; }
        [JsonProperty(PropertyName = "role")]
        public string Role { get; set; }
        [JsonProperty(PropertyName = "unfriendly")]
        public bool Unfriendly { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "title_expire_time")]
        public long TsTitleExpireTime { get; set; }
        [JsonProperty(PropertyName = "card_changeable")]
        public bool CardChangeable { get; set; }

        // 拓展属性
        [JsonIgnore]
        public DateTime JoinTime => new DateTime(1970, 1, 1).ToLocalTime().AddSeconds(TsJoinTime);

        [JsonIgnore]
        public DateTime LastSentTime => new DateTime(1970, 1, 1).ToLocalTime().AddSeconds(TsLastSentTime);

        [JsonIgnore]
        public DateTime TitleExpireTime => new DateTime(1970, 1, 1).ToLocalTime().AddSeconds(TsTitleExpireTime);
    }
}
