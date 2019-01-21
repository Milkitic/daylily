using Newtonsoft.Json;

namespace Daylily.TuLing.RequestModel
{
    /// <summary>
    /// 文本信息
    /// </summary>
    public class InputText
    {
        /// <summary>
        /// 直接输入文本 (1-128字符)
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}