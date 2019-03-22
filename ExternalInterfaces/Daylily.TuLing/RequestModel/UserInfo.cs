using Newtonsoft.Json;

namespace Daylily.TuLing.RequestModel
{
    /// <summary>
    /// 用户参数
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 机器人标识
        /// </summary>
        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }

        /// <summary>
        /// 用户唯一标识
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// 群聊唯一标识
        /// </summary>
        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        /// <summary>
        /// 群内用户昵称
        /// </summary>
        [JsonProperty("userIdName")]
        public string UserIdName { get; set; }
    }
}