using System;
using Newtonsoft.Json;

namespace Daylily.TuLing.ResponseModel
{
    public class Response
    {
        /// <summary>
        /// 请求意图
        /// </summary>
        [JsonProperty("intent")]
        public Intent Intent { get; set; }

        /// <summary>
        /// 输出结果集
        /// </summary>
        [JsonProperty("results")]
        public Result[] Results { get; set; }
    }

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

    /// <summary>
    /// 功能相关参数
    /// </summary>
    public class Parameters
    {
        [JsonProperty("nearby_place")]
        public string NearbyPlace { get; set; }
    }

    /// <summary>
    /// 输出结果集
    /// </summary>
    public class Result
    {
        /// <summary>
        /// ‘组’编号:0为独立输出，大于0时可能包含同组相关内容 (如：音频与文本为一组时说明内容一致)
        /// </summary>
        [JsonProperty("groupType")]
        public long GroupType { get; set; }

        /// <summary>
        /// 输出类型
        /// </summary>
        [JsonProperty("resultType")]
        public string ResultType { get; set; }

        /// <summary>
        /// 输出值
        /// </summary>
        [JsonProperty("values")]
        public Values Values { get; set; }
    }

    public class Values
    {
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Url { get; set; }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
    }
}
