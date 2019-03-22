using Newtonsoft.Json;

namespace Daylily.CoolQ.CoolQHttp.ResponseModel.Report
{
    /// <summary>
    /// 好友添加。
    /// </summary>
    public class FriendAdd
    {
        /// <summary>
        /// 事件名。
        /// </summary>
        [JsonProperty(PropertyName = "notice_type")]
        public string NoticeType { get; set; }
        /// <summary>
        /// 新添加好友 QQ 号。
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public long UserId { get; set; }
    }
}
