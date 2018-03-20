using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DaylilyWeb.Assist
{
    class WebRequestHelper
    {
        /// <summary>
        /// 创建一个一般请求
        /// </summary>
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters = null, int timeout = -1, string userAgent = null, CookieCollection cookies = null)
        {
            HttpWebRequest request = _GetRequestObj(url, parameters, timeout, userAgent, cookies);
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 创建一个异步请求
        /// </summary>
        public static async Task<HttpWebResponse> CreatePostHttpResponseAsync(string url, IDictionary<string, string> parameters = null, int timeout = -1, string userAgent = null, CookieCollection cookies = null)
        {
            HttpWebRequest request = _GetRequestObj(url, parameters, timeout, userAgent, cookies);
            return await request.GetResponseAsync() as HttpWebResponse;
        }

        /// <summary>
        /// 从已创建的请求中获取字符串
        /// </summary>
        public static string GetResponseString(HttpWebResponse webresponse)
        {
            using (Stream s = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }

        private static HttpWebRequest _GetRequestObj(string url, IDictionary<string, string> parameters, int timeout, string userAgent, CookieCollection cookies)
        {
            HttpWebRequest request = null;

            // HTTPS? 保留
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            // 设定UserAgent以及超时
            if (userAgent != null) request.UserAgent = userAgent;
            if (timeout != -1) request.Timeout = timeout;

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }

            // 参数 
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        i++;
                    }
                }
                byte[] data = Encoding.ASCII.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            // 保留使用
            string[] values = request.Headers.GetValues("Content-Type");

            return request;
        }
    }
}
