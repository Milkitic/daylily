using System.Text.Json.Serialization;

namespace daylily.ThirdParty.Tuling.RequestModel
{
    /// <summary>
    /// 文本信息
    /// </summary>
    public class InputText
    {
        /// <summary>
        /// 直接输入文本 (1-128字符)
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}