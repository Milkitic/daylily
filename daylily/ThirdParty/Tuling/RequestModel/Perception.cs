using System.Text.Json.Serialization;

namespace daylily.Tuling.RequestModel
{
    /// <summary>
    /// 输入信息
    /// </summary>
    public class Perception
    {
        /// <summary>
        /// 文本信息
        /// </summary>
        [JsonPropertyName("inputText")]
        public InputText InputText { get; set; }

        /// <summary>
        /// 图片信息
        /// </summary>
        [JsonPropertyName("inputImage")]
        public InputImage InputImage { get; set; }

        /// <summary>
        /// 音频信息
        /// </summary>
        [JsonPropertyName("inputMedia")]
        public InputImage InputMedia { get; set; }

        /// <summary>
        /// 客户端属性
        /// </summary>
        [JsonPropertyName("selfInfo")]
        public SelfInfo SelfInfo { get; set; }
    }
}