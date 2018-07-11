using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Daylily.Common.Utils;

namespace Daylily.Common.Utils.HttpRequest
{
    /// <summary>
    /// Web请求帮助类
    /// </summary>
    public static class WebRequestUtil
    {
        private const int Timeout = 3000; // 整体的一次请求超时时间
        /// <summary>
        /// 从URL中提取图片
        /// </summary>
        /// <param name="url">原始URL地址</param>
        /// <param name="savePath">要保存的文件名（不包含扩展名）</param>
        /// <param name="ext">要保存的扩展名</param>
        /// <returns></returns>
        public static string GetImageFromUrl(string url, string savePath, string ext)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            var response = TryGetResponse(request);

            response.GetResponseStream();
            return !response.ContentType.ToLower().StartsWith("text/") ? SaveBinaryFile(response, savePath, ext) : null;
        }

        /// <summary>
        /// 创建一个POST请求（键值对形式）
        /// </summary>
        /// <param name="url">需要请求的地址</param>
        /// <param name="parameters">请求的参数</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="userAgent"></param>
        /// <param name="cookies">Cookie</param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters = null,
            int timeout = -1, string userAgent = null, CookieCollection cookies = null, string authorization = null)
        {
            // 参数
            StringBuilder buffer = new StringBuilder();
            int i = 0;
            if (parameters != null)
            {
                foreach (var key in parameters.Keys)
                {
                    buffer.AppendFormat(i > 0 ? "&{0}={1}" : "{0}={1}", key, parameters[key]);
                    i++;
                }
            }

            HttpWebRequest request = _GetReqPostObj("application/x-www-form-urlencoded", url, buffer.ToString(),
                timeout, userAgent, cookies, authorization);
            return TryGetResponse(request);
        }

        /// <summary>
        /// 创建一个一般请求
        /// </summary>
        public static HttpWebResponse CreatePostHttpResponse(string url, object jsonObj, int timeout = -1,
            string userAgent = null, CookieCollection cookies = null, string authorization = null)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj);

            HttpWebRequest request =
                _GetReqPostObj("application/json", url, json, timeout, userAgent, cookies, authorization);

            return TryGetResponse(request);
        }

        /// <summary>
        /// 创建一个一般请求
        /// </summary>
        public static HttpWebResponse CreatePostHttpResponse(string url, string json, int timeout = -1,
            string userAgent = null, CookieCollection cookies = null, string authorization = null)
        {
            HttpWebRequest request =
                _GetReqPostObj("application/json", url, json, timeout, userAgent, cookies, authorization);
            return TryGetResponse(request);
        }

        /// <summary>
        /// 创建一个异步请求
        /// </summary>
        public static async Task<HttpWebResponse> CreatePostHttpResponseAsync(string url,
            IDictionary<string, string> parameters = null, int timeout = -1, string userAgent = null,
            CookieCollection cookies = null, string authorization = null)
        {
            // 参数
            StringBuilder buffer = new StringBuilder();
            int i = 0;
            if (parameters != null)
            {
                foreach (var key in parameters.Keys)
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
            }

            HttpWebRequest request = _GetReqPostObj("application/x-www-form-urlencoded", url, buffer.ToString(),
                timeout, userAgent, cookies, authorization);
            return await TryGetResponseAsync(request);
        }

        /// <summary>
        /// 创建一个一般Get请求
        /// </summary>
        public static HttpWebResponse CreateGetHttpResponse(string url, IDictionary<string, string> parameters = null)
        {
            HttpWebRequest request = _GetReqGetObj(url, parameters);
            return TryGetResponse(request);
        }

        /// <summary>
        /// 创建一个一般Get请求
        /// </summary>
        public static HttpWebResponse CreateUrlGetHttpResponse(string url,
            IDictionary<string, string> parameters = null)
        {
            HttpWebRequest request = _GetReqUrlGetObj(url, parameters);
            return TryGetResponse(request);
        }

        /// <summary>
        /// 从已创建的请求中获取字符串
        /// </summary>
        /// <param name="webresponse">已创建的请求</param>
        /// <returns></returns>
        public static string GetResponseString(HttpWebResponse webresponse)
        {
            using (Stream s = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s ?? throw new InvalidOperationException(), Encoding.UTF8);
                //webresponse.Close();
                return reader.ReadToEnd();
            }
        }

        private static HttpWebRequest _GetReqPostObj(string contentType, string url, string param, int timeout,
            string userAgent, CookieCollection cookies, string authorization)
        {
            HttpWebRequest request;

            // HTTPS? 保留
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            if (request == null) throw new NullReferenceException();

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
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
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
            // string[] values = request.Headers.GetValues("Content-Type");

            return request;
        }

        private static HttpWebRequest _GetReqGetObj(string url, IDictionary<string, string> parameters)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

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
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            // 保留使用
            // string[] values = request.Headers.GetValues("Content-Type");

            return request;
        }

        private static HttpWebRequest _GetReqUrlGetObj(string url, IDictionary<string, string> parameters)
        {
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (var key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("?{0}={1}", key, parameters[key]);
                        i++;
                    }
                }

                url = url + buffer;
            }

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            return request;
        }

        private static string SaveBinaryFile(WebResponse response, string savePath, string ext)
        {
            byte[] buffer = new byte[1024];

            var imagePath = Path.Combine(Domain.CurrentDirectory, "images");
            var filePath = Path.Combine(imagePath, savePath + ext);

            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                Stream outStream = File.Create(filePath);
                Stream inStream = response.GetResponseStream();
                if (inStream == null) throw new NullReferenceException();

                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0)
                        outStream.Write(buffer, 0, l);
                } while (l > 0);

                outStream.Close();
                inStream.Close();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return null;
            }

            return filePath;
        }

        private static HttpWebResponse TryGetResponse(WebRequest request)
        {
            Stopwatch sw = new Stopwatch();
            request.Timeout = Timeout;
            HttpWebResponse response = null;
            const int count = 3;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    //sw.Restart();
                    response = request.GetResponse() as HttpWebResponse;
                    //sw.Stop();
                    //Logger.SuccessLine($"{request.RequestUri.AbsoluteUri}: {sw.ElapsedMilliseconds}ms");
                    break;
                }
                catch (Exception)
                {
                    Logger.Error($"尝试了{i}次，请求超时 (>{request.Timeout}ms)");
                    Logger.Error($"{request.RequestUri.AbsoluteUri}: {sw.ElapsedMilliseconds}ms");
                    if (i == count - 1)
                        throw;
                }
            }

            sw.Stop();
            return response;
        }

        private static async Task<HttpWebResponse> TryGetResponseAsync(WebRequest request)
        {
            request.Timeout = Timeout;
            HttpWebResponse response = null;
            const int count = 3;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    response = await request.GetResponseAsync() as HttpWebResponse;
                    break;
                }
                catch (Exception)
                {
                    Logger.Error($"尝试了{i}次，请求超时 (>{request.Timeout}ms)");
                    if (i == count - 1)
                        throw;
                }
            }

            return response;
        }
    }
}
