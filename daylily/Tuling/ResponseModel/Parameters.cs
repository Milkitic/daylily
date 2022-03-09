using System.Text.Json.Serialization;

namespace daylily.Tuling.ResponseModel
{
    /// <summary>
    /// 功能相关参数
    /// </summary>
    public class Parameters
    {
        [JsonPropertyName("nearby_place")]
        public string NearbyPlace { get; set; }
    }
}