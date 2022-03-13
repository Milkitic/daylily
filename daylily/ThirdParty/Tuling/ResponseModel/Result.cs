using System.Text.Json.Serialization;

namespace daylily.ThirdParty.Tuling.ResponseModel
{
    /// <summary>
    /// 输出结果集
    /// </summary>
    public class Result
    {
        /// <summary>
        /// ‘组’编号:0为独立输出，大于0时可能包含同组相关内容 (如：音频与文本为一组时说明内容一致)
        /// </summary>
        [JsonPropertyName("groupType")]
        public long GroupType { get; set; }

        /// <summary>
        /// 输出类型
        /// </summary>
        [JsonPropertyName("resultType")]
        public string ResultType { get; set; }

        /// <summary>
        /// 输出值
        /// </summary>
        [JsonPropertyName("values")]
        public Values Values { get; set; }
    }
}