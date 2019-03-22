using Newtonsoft.Json;

namespace Daylily.TuLing.RequestModel
{
    /// <summary>
    /// 输入信息
    /// </summary>
    public class Perception
    {
        /// <summary>
        /// 文本信息
        /// </summary>
        [JsonProperty("inputText")]
        public InputText InputText { get; set; }

        /// <summary>
        /// 图片信息
        /// </summary>
        [JsonProperty("inputImage")]
        public InputImage InputImage { get; set; }

        /// <summary>
        /// 音频信息
        /// </summary>
        [JsonProperty("inputMedia")]
        public InputImage InputMedia { get; set; }

        /// <summary>
        /// 客户端属性
        /// </summary>
        [JsonProperty("selfInfo")]
        public SelfInfo SelfInfo { get; set; }
    }
}