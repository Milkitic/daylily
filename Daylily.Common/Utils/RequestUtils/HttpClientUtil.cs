using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Daylily.Common.IO;
using Daylily.Common.Utils.LoggerUtils;

namespace Daylily.Common.Utils.RequestUtils
{
    public static class HttpClientUtil
    {
        public static bool EnableLog { get; set; } = false;
        public static int Timeout { get; set; } = 8000;
        public static int RetryCount { get; set; } = 3;
        private static readonly HttpClient Http;

        static HttpClientUtil()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip
            };
            ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
            Http = new HttpClient(handler)
            {
                Timeout = new TimeSpan(0, 0, 0, 0, Timeout),
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
        /// <param name="argsHeader">请求头字典</param>
        /// <returns></returns>
        public static string HttpGet(string url, IDictionary<string, string> args = null, IDictionary<string, string> argsHeader = null)
        {
            for (int i = 0; i < RetryCount; i++)
            {
                try
                {
                    if (args != null)
                    {
                        url = url + args.ToUrlParamString();
                    }

                    var message = new HttpRequestMessage(HttpMethod.Get, url);
                    if (argsHeader != null)
                    {
                        foreach (var item in argsHeader)
                        {
                            message.Headers.Add(item.Key, item.Value);
                        }
                    }
                    CancellationTokenSource cts = new CancellationTokenSource(Timeout);
                    HttpResponseMessage response = Http.SendAsync(message, cts.Token).Result;

                    return response.Content.ReadAsStringAsync().Result;
                }
                catch (Exception ex)
                {
                    Logger.Error($"尝试了{i + 1}次，请求超时 (>{Timeout}ms): " + url);
                    if (i == RetryCount - 1)
                        Logger.Exception(ex);
                }
            }

            return null;
        }

        public static Image GetImageFromUrl(string url)
        {
            Uri uri = new Uri(Uri.EscapeUriString(url));
            byte[] urlContents = Http.GetByteArrayAsync(uri).Result;
            string fullName = Path.Combine(Domain.CacheImagePath, Guid.NewGuid().ToString());
            FileStream fs = new FileStream(fullName, FileMode.OpenOrCreate);
            fs.Write(urlContents, 0, urlContents.Length);
            return Image.FromStream(fs);
        }

        public static string SaveImageFromUrl(string url, ImageFormat format, string filename = null, string savePath = null)
        {
            Contract.Requires<NotSupportedException>(Equals(format, ImageFormat.Jpeg) ||
                                                     Equals(format, ImageFormat.Png) ||
                                                     Equals(format, ImageFormat.Gif));
            var img = GetImageFromUrl(url);
            string imgPath = Domain.CacheImagePath;
            filename = filename ?? Guid.NewGuid().ToString();
            savePath = savePath ?? imgPath;

            string ext = "";
            if (Equals(format, ImageFormat.Jpeg))
                ext = ".jpg";
            else if (Equals(format, ImageFormat.Png))
                ext = ".png";
            else if (Equals(format, ImageFormat.Gif))
                ext = ".gif";
            string fullname = Path.Combine(savePath, filename + ext);
            img.Save(fullname, format);
            return new FileInfo(fullname).FullName;
        }

        private static string HttpPost(string url, HttpContent content)
        {
            string responseStr = null;
            for (int i = 0; i < RetryCount; i++)
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
                    responseStr = response.Content.ReadAsStringAsync().Result;
                    return responseStr;
                }
                catch (Exception ex)
                {
                    Logger.Error($"尝试了{i + 1}次，请求超时 (>{Timeout}ms): " + url);
                    if (i == RetryCount - 1)
                        Logger.Exception(ex);
                }
            }

            return responseStr;
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
