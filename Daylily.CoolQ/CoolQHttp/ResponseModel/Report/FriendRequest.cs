using Newtonsoft.Json;

namespace Daylily.CoolQ.CoolQHttp.ResponseModel.Report
{
    /// <summary>
    /// 加好友请求。
    /// </summary>
    public class FriendRequest
    {
        /// <summary>
        /// 请求类型。
        /// </summary>
        [JsonProperty(PropertyName = "request_type")]
        public string RequestType { get; set; }
        /// <summary>
        /// 发送请求的 QQ 号。
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public long UserId { get; set; }
        /// <summary>
        /// 验证信息。
        /// </summary>
        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }
        /// <summary>
        /// 请求 flag，在调用处理请求的 API 时需要传入。
        /// </summary>
        [JsonProperty(PropertyName = "flag")]
        public long Flag { get; set; }
    }

    /// <summary>
    /// 加好友请求的响应。
    /// </summary>
    public class FriendRequestResp
    {
        /// <summary>
        /// 是否同意请求。
        /// </summary>
        [JsonProperty(PropertyName = "approve")]
        public bool Approve { get; set; }
        /// <summary>
        /// 添加后的好友备注（仅在同意时有效）。
        /// </summary>
        [JsonProperty(PropertyName = "remark")]
        public string Remark { get; set; }
    }
}
