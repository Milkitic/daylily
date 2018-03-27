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
        public static string ApiUrl { get; set; } = "http://www.mothership.top:5700";
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

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/send_private_msg", parameters);
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

            var response = WebRequestHelper.CreatePostHttpResponseAsync(ApiUrl + "/send_private_msg_async", parameters);
            Log.DefaultLine("Sent request. (PrivateMessageAsync)", ToString());
            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(await response);
                Log.DefaultLine("Received response. (PrivateMessageAsync)", ToString());
            }
            return json_string;
        }
        /// <summary>
        /// 发送群聊消息
        /// </summary>
        /// <param name="id">群号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public string SendGroupMessage(string groupId, string message)
        {
            string json_string = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("group_id", HttpUtility.UrlEncode(groupId));
            parameters.Add("message", HttpUtility.UrlEncode(message));

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/send_group_msg", parameters);
            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(response);
            }
            return json_string;
        }

        /// <summary>
        /// 发送群聊消息（异步版本）
        /// </summary>
        /// <param name="id">群号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public async Task<string> SendGroupMessageAsync(string groupId, string message)
        {
            string json_string = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("group_id", HttpUtility.UrlEncode(groupId));
            parameters.Add("message", HttpUtility.UrlEncode(message));

            var response = WebRequestHelper.CreatePostHttpResponseAsync(ApiUrl + "/send_group_msg_async", parameters);
            Log.DefaultLine("Sent request. (GroupMessageAsync)", ToString());
            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(await response);
                Log.DefaultLine("Received response. (GroupMessageAsync)", ToString());
            }
            return json_string;
        }

        public Models.CQResponse.GroupList GetGroupList()
        {
            string json_string = null;

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/get_group_list");
            Log.DefaultLine("Sent request. (GetGroupList)", ToString());
            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(response);
                Log.DefaultLine("Received response. (GroupMessageAsync)", ToString());
                Log.DefaultLine(json_string, ToString());
            }
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.CQResponse.GroupList>(json_string);
            return obj;
        }
    }
}
