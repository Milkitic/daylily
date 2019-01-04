using Newtonsoft.Json;

namespace Daylily.CoolQ.CoolQHttp.ResponseModel.Report
{
    /// <summary>
    /// 群管理员变动。
    /// </summary>
    public class GroupAdminChange
    {
        /// <summary>
        /// 事件名。
        /// </summary>
        [JsonProperty(PropertyName = "notice_type")]
        public string NoticeType { get; set; }
        /// <summary>
        /// 事件子类型，分别表示设置和取消管理员。
        /// </summary>
        [JsonProperty(PropertyName = "sub_type")]
        public string SubType { get; set; }
        /// <summary>
        /// 群号。
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; set; }
        /// <summary>
        /// 管理员 QQ 号。
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public long UserId { get; set; }
    }
}
