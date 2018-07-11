using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Daylily.Common.Utils.HttpRequest
{
    public static class HttpClientUtil
    {
        public static bool EnableLog { get; set; } = false;
        private static readonly HttpClient Http;
        static HttpClientUtil()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip
            };
            ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
            Http = new HttpClient()
            {
                Timeout = new TimeSpan(0, 0, 3)
            };
        }

        /// <summary>
        /// POST 无参数
        /// </summary>
        /// <param name="url">Http地址</param>
        /// <returns></returns>
        public static string HttpPost(string url)
        {
            HttpContent content = new StringContent("");
            content.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.Form.GetContentType());
            return HttpPost(url, content);
        }

        /// <summary>
        /// POST 提交json格式参数
        /// </summary>
        /// <param name="url">Http地址</param>
        /// <param name="postJson">json字符串</param>
        /// <returns></returns>
        public static string HttpPost(string url, string postJson)
        {
            HttpContent content = new StringContent(postJson);
            content.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.Json.GetContentType());
            return HttpPost(url, content);
        }

        /// <summary>
        /// POST 提交json格式参数
        /// </summary>
        /// <param name="url">Http地址</param>
        /// <param name="args">参数字典</param>
        /// <param name="argsHeader">请求头字典</param>
        /// <returns></returns>
        public static string HttpPost(string url, IDictionary<string, string> args, IDictionary<string, string> argsHeader = null)
        {
            HttpContent content;
            //argDic.ToSortUrlParamString();
            if (args != null)
            {
                var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(args);
                content = new StringContent(jsonStr);
                content.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.Json.GetContentType());
            }
            else
            {
                content = new StringContent("");
                content.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.Form.GetContentType());
            }
            if (argsHeader != null)
            {
                foreach (var item in argsHeader)
                    content.Headers.Add(item.Key, item.Value);
            }

            return HttpPost(url, content);
        }

        /// <summary>
        /// GET 请求
        /// </summary>
        /// <param name="url">Http地址</param>
        /// <param name="args">参数字典</param>
        /// <param name="headerDic">请求头字典</param>
        /// <returns></returns>
        public static string HttpGet(string url, IDictionary<string, string> args = null, IDictionary<string, string> headerDic = null)
        {
            try
            {
                if (args != null)
                {
                    url = url + args.ToUrlParamString();
                }

                if (headerDic != null)
                {
                    foreach (var item in headerDic)
                    {
                        Http.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

                //await异步等待回应
                if (EnableLog)
                    Logger.Debug("Sent get request.");
                var response = Http.GetStringAsync(url).Result;
                if (EnableLog)
                    Logger.Debug("Received get response.");
                return response;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                throw;
            }
        }

        private static string HttpPost(string url, HttpContent content)
        {
            try
            {
                if (EnableLog)
                    Logger.Debug("Sent post request.");
                var response = Http.PostAsync(url, content).Result;
                if (EnableLog)
                    Logger.Debug("Received post response.");

                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();
                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                var reJson = response.Content.ReadAsStringAsync().Result;
                return reJson;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                throw;
            }
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }

        private static string ToUrlParamString(this IDictionary<string, string> args)
        {
            if (args == null || args.Count <= 1)
                return "";
            StringBuilder sb = new StringBuilder("?");
            foreach (var item in args)
                sb.Append(item.Key + "=" + item.Value + "&");
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        private static string GetContentType(this HttpContentType type)
        {
            switch (type)
            {
                case HttpContentType.Json:
                    return "application/json";
                default:
                case HttpContentType.Form:
                    return "application/x-www-form-urlencoded";
            }
        }

        private enum HttpContentType
        {
            Json,
            Form
        }
    }
}
