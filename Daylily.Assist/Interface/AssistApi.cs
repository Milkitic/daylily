using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using Daylily.Common.Utils.RequestUtils;

namespace Daylily.Assist.Interface
{
    public static class AssistApi
    {
        public static string ApiUrl { get; set; } = "http://139.199.103.11:23334";

        public static string GetImgFile(string fileName)
        {
            string str = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "fileName", HttpUtility.UrlEncode(fileName) }
            };

            var response = WebRequestUtil.CreatePostHttpResponse(ApiUrl + "/api/imgfile", parameters);
            if (response != null)
            {
                str = WebRequestUtil.GetResponseString(response);
            }

            return str;
        }

        public static Image GetStrokeString(string str)
        {
            return HttpClientUtil.GetImageFromUrl(ApiUrl + "/api/strokestring?str=" + str);
        }
    }
}
