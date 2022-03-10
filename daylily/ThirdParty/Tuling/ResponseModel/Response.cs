using System.Text.Json.Serialization;

namespace daylily.Tuling.ResponseModel
{
    public class Response
    {
        /// <summary>
        /// 请求意图
        /// </summary>
        [JsonPropertyName("intent")]
        public Intent Intent { get; set; }

        /// <summary>
        /// 输出结果集
        /// </summary>
        [JsonPropertyName("results")]
        public Result[] Results { get; set; }
    }
}
