using System.Text.Json.Serialization;

namespace daylily.Tuling.RequestModel
{
    /// <summary>
    /// 用户参数
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 机器人标识
        /// </summary>
        [JsonPropertyName("apiKey")]
        public string ApiKey { get; set; }

        /// <summary>
        /// 用户唯一标识
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// 群聊唯一标识
        /// </summary>
        [JsonPropertyName("groupId")]
        public string GroupId { get; set; }

        /// <summary>
        /// 群内用户昵称
        /// </summary>
        [JsonPropertyName("userIdName")]
        public string? UserIdName { get; set; }
    }
}