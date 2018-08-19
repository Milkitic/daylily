using System.Collections.Generic;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.Common.Utils.RequestUtils;
using Daylily.Osu.Models;

namespace Daylily.Osu.Interface
{
    public class EloApi
    {
        public static string ApiUrl { get; set; } = "http://elo.milkitic.name";

        public EloUserInfo GetEloByUid(long uid)
        {
            string jsonString = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"user_id", uid.ToString()}
            };

            var response = WebRequestUtil.CreateUrlGetHttpResponse(ApiUrl + "/user", parameters);
            Logger.Debug("Sent request.");

            if (response != null)
            {
                jsonString = WebRequestUtil.GetResponseString(response);
                Logger.Debug("Received response.");
            }

            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<EloUserInfo>(jsonString);
            return obj;
        }
    }
}
