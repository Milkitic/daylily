using System.Text.Json.Serialization;

namespace daylily.ThirdParty.Tuling.RequestModel
{
    internal class Request
    {
        /// <summary>
        /// 输入类型
        /// </summary>
        [JsonPropertyName("reqType")]
        public RequestType RequestType { get; set; }

        /// <summary>
        /// 输入信息
        /// </summary>
        [JsonPropertyName("perception")]
        public Perception Perception { get; set; }

        /// <summary>
        /// 用户参数
        /// </summary>
        [JsonPropertyName("userInfo")]
        public UserInfo UserInfo { get; set; }
    }
}
