using Daylily.Common.Assist;
using System.Collections.Generic;
using System.Web;

namespace Daylily.Common.Interface.DaylilyAssist
{
    public class AssistApi
    {
        public static string ApiUrl { get; set; } = "http://123.207.137.177:23334";

        public static string GetImgFile(string fileName)
        {
            string str = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("fileName", HttpUtility.UrlEncode(fileName));

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/api/imgfile", parameters);
            if (response != null)
            {
                str = WebRequestHelper.GetResponseString(response);
            }

            return str;
        }

    }
}
