using DaylilyWeb.Assist;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DaylilyWeb.Interface.CQHttp
{
    class HttpApi
    {
        /// <summary>
        /// 发送私聊消息
        /// </summary>
        /// <param name="id">对方 QQ 号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public string SendPrivateMessage(string id, string message)
        {
            string json_string = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("user_id", HttpUtility.UrlEncode(id));
            parameters.Add("message", HttpUtility.UrlEncode(message));

            var response = WebRequestHelper.CreatePostHttpResponse("http://127.0.0.1:5700/send_private_msg", parameters);
            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(response);
            }
            return json_string;
        }

        /// <summary>
        /// 发送私聊消息（异步版本）
        /// </summary>
        /// <param name="id">对方 QQ 号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public async Task<string> SendPrivateMessageAsync(string id, string message)
        {
            string json_string = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("user_id", HttpUtility.UrlEncode(id));
            parameters.Add("message", HttpUtility.UrlEncode(message));

            var response = WebRequestHelper.CreatePostHttpResponseAsync("http://127.0.0.1:5700/send_private_msg_async", parameters);
            Log.WriteLine("Sent request.", ToString());
            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(await response);
                Log.WriteLine("Received response.", ToString());
            }
            return json_string;
        }

    }
}
