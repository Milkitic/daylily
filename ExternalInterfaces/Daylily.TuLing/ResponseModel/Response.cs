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
}
