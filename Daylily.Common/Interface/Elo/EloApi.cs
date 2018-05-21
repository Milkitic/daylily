using Daylily.Common.Assist;
using Daylily.Common.Models.Elo.Api;
using System.Collections.Generic;

namespace Daylily.Common.Interface.Elo
{
    public class EloApi
    {
        public static string ApiUrl { get; set; } = "http://elo.milkitic.name";

        public EloUserInfo GetEloByUid(long uid)
        {
            string jsonString = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "user_id", uid.ToString() }
            };

            var response = WebRequestHelper.CreateUrlGetHttpResponse(ApiUrl + "/user", parameters);
            Logger.DefaultLine("Sent request.");

            if (response != null)
            {
                jsonString = WebRequestHelper.GetResponseString(response);
                Logger.DefaultLine("Received response.");
            }
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<EloUserInfo>(jsonString);
            return obj;
        }
    }
}
