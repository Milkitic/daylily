using Daylily.Common.Web;
using System.Collections.Generic;

namespace Daylily.Osu.Elo
{
    public static class EloApiClient
    {
        public static string ApiUrl { get; set; } = "http://elo.milkitic.name";

        public static EloUserInfo GetEloByUid(long uid)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"user_id", uid.ToString()}
            };

            string jsonString = HttpClient.HttpGet(ApiUrl + "/user", parameters);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<EloUserInfo>(jsonString);
        }
    }
}
