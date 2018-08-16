using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using Daylily.Common.Utils.HttpRequest;

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

        public static Bitmap GetStrokeString(string str)
        {
            var response = WebRequestUtil.GetImageFromUrl(ApiUrl + "/api/strokestring?str=" + str, Guid.NewGuid().ToString(), "png");
            return new Bitmap(response);
        }
    }
}
