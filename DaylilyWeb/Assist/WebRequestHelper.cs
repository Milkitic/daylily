using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DaylilyWeb.Assist
{
    class WebRequestHelper
    {
        static Random rnd = new Random();

        public static string GetImageFromUrl(string url, string savePath, string ext)
        {
            WebResponse response = null;
            Stream stream = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                response = request.GetResponse();
                stream = response.GetResponseStream();

                if (!response.ContentType.ToLower().StartsWith("text/"))
                {
                    return SaveBinaryFile(response,  savePath, ext);
                }

            }
            catch (Exception e)
            {
                string em = e.ToString();
            }
            return null;
        }
        /// <summary>
        /// 创建一个一般请求
        /// </summary>
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters = null, int timeout = -1, string userAgent = null, CookieCollection cookies = null, string authorization = null)
        {
            // 参数
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

            HttpWebRequest request = _GetReqPostObj("application/x-www-form-urlencoded", url, buffer.ToString(), timeout, userAgent, cookies, authorization);
            return request.GetResponse() as HttpWebResponse;
        }
        /// <summary>
        /// 创建一个一般请求
        /// </summary>
        public static HttpWebResponse CreatePostHttpResponse(string url, object jsonObj, int timeout = -1, string userAgent = null, CookieCollection cookies = null, string authorization = null)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj);

            HttpWebRequest request = _GetReqPostObj("application/json", url, json, timeout, userAgent, cookies, authorization);
            return request.GetResponse() as HttpWebResponse;
        }
        /// <summary>
        /// 创建一个一般请求
        /// </summary>
        public static HttpWebResponse CreatePostHttpResponse(string url, string json, int timeout = -1, string userAgent = null, CookieCollection cookies = null, string authorization = null)
        {
            HttpWebRequest request = _GetReqPostObj("application/json", url, json, timeout, userAgent, cookies, authorization);
            return request.GetResponse() as HttpWebResponse;
        }
        /// <summary>
        /// 创建一个异步请求
        /// </summary>
        public static async Task<HttpWebResponse> CreatePostHttpResponseAsync(string url, IDictionary<string, string> parameters = null, int timeout = -1, string userAgent = null, CookieCollection cookies = null, string authorization = null)
        {
            // 参数
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

            HttpWebRequest request = _GetReqPostObj("application/x-www-form-urlencoded", url, buffer.ToString(), timeout, userAgent, cookies, authorization);
            return await request.GetResponseAsync() as HttpWebResponse;
        }

        /// <summary>
        /// 创建一个一般Get请求
        /// </summary>
        public static HttpWebResponse CreateGetHttpResponse(string url, IDictionary<string, string> parameters = null)
        {
            HttpWebRequest request = _GetReqGetObj(url, parameters);
            return request.GetResponse() as HttpWebResponse;
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

        private static HttpWebRequest _GetReqPostObj(string contentType, string url, string param, int timeout, string userAgent, CookieCollection cookies, string authorization)
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
            // 设定UserAgent以及超时
            if (userAgent != null) request.UserAgent = userAgent;
            if (timeout != -1) request.Timeout = timeout;

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }


            // 参数 
            byte[] data = Encoding.ASCII.GetBytes(param);
            //request.ContentLength = data.Length;
            try
            {
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            catch
            {
                throw new Exception("网络连接失败");
            }
            if (authorization != null)
            {
                request.Headers.Add("authorization", authorization);
                request.Headers.Add("host", "service.image.myqcloud.com");
                request.Headers.Add("content-length", data.Length.ToString());
                request.Headers.Add("content-type", contentType);
            }
            else
            {
                request.ContentType = contentType;
            }
            // 保留使用
            string[] values = request.Headers.GetValues("Content-Type");

            return request;
        }

        private static HttpWebRequest _GetReqGetObj(string url, IDictionary<string, string> parameters)
        {
            HttpWebRequest request = null;

            request = WebRequest.Create(url) as HttpWebRequest;

            //request.ContentType = "application/x-www-form-urlencoded";

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
                try
                {
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                catch
                {
                    throw new Exception("网络连接失败");
                }
            }

            // 保留使用
            string[] values = request.Headers.GetValues("Content-Type");

            return request;
        }

        private static string SaveBinaryFile(WebResponse response, string savePath, string ext)
        {
            byte[] buffer = new byte[1024];

            var imagePath = Path.Combine(Environment.CurrentDirectory, "images");
            var filePath = Path.Combine(imagePath, savePath + ext);

            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                Stream outStream = File.Create(filePath);
                Stream inStream = response.GetResponseStream();

                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0)
                        outStream.Write(buffer, 0, l);
                }
                while (l > 0);

                outStream.Close();
                inStream.Close();
            }
            catch
            {
                return null;
            }
            return filePath;
        }
    }
}
