using Newtonsoft.Json;

namespace Daylily.TuLing.ResponseModel
{
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
}