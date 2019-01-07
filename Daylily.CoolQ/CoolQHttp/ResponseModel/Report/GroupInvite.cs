using Newtonsoft.Json;

namespace Daylily.CoolQ.CoolQHttp.ResponseModel.Report
{
    /// <summary>
    /// 加群请求／邀请。
    /// </summary>
    public class GroupInvite : ReportBase
    {
        /// <summary>
        /// 请求类型。
        /// </summary>
        [JsonProperty(PropertyName = "request_type")]
        public string RequestType { get; set; }

        /// <summary>
        /// 请求子类型，add、invite分别表示加群请求、邀请登录号入群。
        /// </summary>
        [JsonProperty(PropertyName = "sub_type")]
        public string SubType { get; set; }

        /// <summary>
        /// 群号。
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; set; }

        /// <summary>
        /// 发送请求的 QQ 号。
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public long UserId { get; set; }

        /// <summary>
        /// 验证信息。
        /// </summary>
        [JsonProperty(PropertyName = "comment")]
        public long Comment { get; set; }

        /// <summary>
        /// 请求 flag，在调用处理请求的 API 时需要传入。
        /// </summary>
        [JsonProperty(PropertyName = "flag")]
        public long Flag { get; set; }
    }

    /// <summary>
    /// 加群请求／邀请的响应。
    /// </summary>
    public class GroupInviteResp
    {
        /// <summary>
        /// 是否同意请求／邀请。
        /// </summary>
        [JsonProperty(PropertyName = "approve")]
        public bool Approve { get; set; }

        /// <summary>
        /// 拒绝理由（仅在拒绝时有效）。
        /// </summary>
        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }
    }
}
