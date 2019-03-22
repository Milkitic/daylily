using System.Collections.Generic;
using System.Drawing;
using System.Web;
using Daylily.Common.Web;

namespace Daylily.Assist
{
    public static class AssistApiClient
    {
        public static string ApiUrl { get; set; } = "http://139.199.103.11:23334";

        public static string GetImgFile(string fileName)
        {
            string str = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "fileName", HttpUtility.UrlEncode(fileName) }
            };

            var response = WebRequest.CreatePostHttpResponse(ApiUrl + "/api/imgfile", parameters);
            if (response != null)
            {
                str = WebRequest.GetResponseString(response);
            }

            return str;
        }

        public static Image GetStrokeString(string str)
        {
            return HttpClient.GetImageFromUrl(ApiUrl + "/api/strokestring?str=" + str);
        }
    }
}
