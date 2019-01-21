using Newtonsoft.Json;

namespace Daylily.TuLing.RequestModel
{
    internal class Request
    {
        /// <summary>
        /// 输入类型
        /// </summary>
        [JsonProperty("reqType")]
        public RequestType RequestType { get; set; }

        /// <summary>
        /// 输入信息
        /// </summary>
        [JsonProperty("perception")]
        public Perception Perception { get; set; }

        /// <summary>
        /// 用户参数
        /// </summary>
        [JsonProperty("userInfo")]
        public UserInfo UserInfo { get; set; }
    }
}
