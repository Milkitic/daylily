using System.Collections.Generic;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.Common.Utils.RequestUtils;
using Daylily.Osu.Models;

namespace Daylily.Osu.Interface
{
    public static class EloApi
    {
        public static string ApiUrl { get; set; } = "http://elo.milkitic.name";

        public static EloUserInfo GetEloByUid(long uid)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"user_id", uid.ToString()}
            };

            string jsonString = HttpClientUtil.HttpGet(ApiUrl + "/user", parameters);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<EloUserInfo>(jsonString);
        }
    }
}
