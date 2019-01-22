using Newtonsoft.Json;

namespace Daylily.TuLing.ResponseModel
{
    /// <summary>
    /// 请求意图
    /// </summary>
    public class Intent
    {
        /// <summary>
        /// 输出功能code
        /// </summary>
        [JsonProperty("code")]
        public long Code { get; set; }

        /// <summary>
        /// 意图名称
        /// </summary>
        [JsonProperty("intentName")]
        public string IntentName { get; set; }

        /// <summary>
        /// 意图动作名称
        /// </summary>
        [JsonProperty("actionName")]
        public string ActionName { get; set; }

        /// <summary>
        /// 功能相关参数
        /// </summary>
        [JsonProperty("parameters")]
        public Parameters Parameters { get; set; }
    }
}