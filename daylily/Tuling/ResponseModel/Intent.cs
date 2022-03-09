using System.Text.Json.Serialization;

namespace daylily.Tuling.ResponseModel
{
    /// <summary>
    /// 请求意图
    /// </summary>
    public class Intent
    {
        /// <summary>
        /// 输出功能code
        /// </summary>
        [JsonPropertyName("code")]
        public long Code { get; set; }

        /// <summary>
        /// 意图名称
        /// </summary>
        [JsonPropertyName("intentName")]
        public string IntentName { get; set; }

        /// <summary>
        /// 意图动作名称
        /// </summary>
        [JsonPropertyName("actionName")]
        public string ActionName { get; set; }

        /// <summary>
        /// 功能相关参数
        /// </summary>
        [JsonPropertyName("parameters")]
        public Parameters Parameters { get; set; }
    }
}