using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Daylily.Common.Models.CQResponse.Api
{
    public class GroupInfoV2
    {
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; set; }
        [JsonProperty(PropertyName = "group_name")]
        public string GroupName { get; set; }
        [JsonProperty(PropertyName = "create_time")]
        public long CreateTime { get; set; }
        [JsonProperty(PropertyName = "category")]
        public int Category { get; set; }
        [JsonProperty(PropertyName = "member_count")]
        public int MemberCount { get; set; }
        [JsonProperty(PropertyName = "introduction")]
        public string Introduction { get; set; }
        [JsonProperty(PropertyName = "admins")]
        public List<GroupInfoV2Admins> Admins { get; set; }
    }

    public abstract class GroupInfoV2Admins
    {
        [JsonProperty(PropertyName = "user_id")]
        public long UserId { get; set; }
        [JsonProperty(PropertyName = "nickname")]
        public string Nickname { get; set; }
        [JsonProperty(PropertyName = "role")]
        public string Role { get; set; }
    }
}
