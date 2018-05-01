using DaylilyWeb.Assist;
using DaylilyWeb.Models.Elo.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DaylilyWeb.Interface.Elo
{
    public class EloApi
    {
        public static string ApiUrl { get; set; } = "http://elo.milkitic.name";

        public EloUserInfo GetEloByUid(long uid)
        {
            string json_string = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("user_id", uid.ToString());

            var response = WebRequestHelper.CreateUrlGetHttpResponse(ApiUrl + "/user", parameters);
            Logger.DefaultLine("Sent request.");

            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(response);
                Logger.DefaultLine("Received response.");
            }
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<EloUserInfo>(json_string);
            return obj;
        }
    }
}
