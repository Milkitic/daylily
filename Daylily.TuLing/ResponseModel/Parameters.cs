using Newtonsoft.Json;

namespace Daylily.TuLing.ResponseModel
{
    /// <summary>
    /// 功能相关参数
    /// </summary>
    public class Parameters
    {
        [JsonProperty("nearby_place")]
        public string NearbyPlace { get; set; }
    }
}