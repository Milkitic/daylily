using Newtonsoft.Json;

namespace Daylily.CoolQ.Models.CqResponse
{
    /// <summary>
    /// 群成员增加/减少。
    /// </summary>
    public class GroupMemberChange : Base
    {
        /// <summary>
        /// 事件名。
        /// </summary>
        [JsonProperty(PropertyName = "notice_type")]
        public string NoticeType { get; set; }

        /// <summary>
        /// 事件子类型，当增加时approve、invite分别表示管理员已同意入群、管理员邀请入群。
        /// 当减少时leave、kick、kick_me分别表示主动退群、成员被踢、登录号被踢。
        /// </summary>
        [JsonProperty(PropertyName = "sub_type")]
        public string SubType { get; set; }

        /// <summary>
        /// 群号。
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; set; }

        /// <summary>
        /// 操作者 QQ 号。
        /// </summary>
        [JsonProperty(PropertyName = "operator_id")]
        public long OperatorId { get; set; }

        /// <summary>
        /// 加入/离开者 QQ 号。
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public long UserId { get; set; }
    }
}
