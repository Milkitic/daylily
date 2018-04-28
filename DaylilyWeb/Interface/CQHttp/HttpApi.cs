using DaylilyWeb.Assist;
using DaylilyWeb.Models.CQRequest;
using DaylilyWeb.Models.CQResponse;
using DaylilyWeb.Models.CQResponse.Api;
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
        public SendPrivateMsgResponse SendPrivateMessage(string id, string message)
        {
            string json_string = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("user_id", HttpUtility.UrlEncode(id));
            parameters.Add("message", HttpUtility.UrlEncode(message));

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/send_private_msg", parameters);
            Logger.DefaultLine("Sent request.");

            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(response);
                Logger.DefaultLine("Received response.");
            }
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SendPrivateMsgResponse>(json_string);
            return obj;
        }

        /// <summary>
        /// 发送私聊消息（异步版本）
        /// </summary>
        /// <param name="id">对方 QQ 号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public async Task<SendPrivateMsgResponse> SendPrivateMessageAsync(string id, string message)
        {
            string json_string = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("user_id", HttpUtility.UrlEncode(id));
            parameters.Add("message", HttpUtility.UrlEncode(message));

            var response = WebRequestHelper.CreatePostHttpResponseAsync(ApiUrl + "/send_private_msg_async", parameters);
            Logger.DefaultLine("Sent request.");

            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(await response);
                Logger.DefaultLine("Received response.");
            }
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SendPrivateMsgResponse>(json_string);
            return obj;
        }
        /// <summary>
        /// 发送群聊消息
        /// </summary>
        /// <param name="id">群号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public SendGroupMsgResponse SendGroupMessage(string groupId, string message)
        {
            string json_string = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("group_id", HttpUtility.UrlEncode(groupId));
            parameters.Add("message", HttpUtility.UrlEncode(message));

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/send_group_msg", parameters);
            Logger.DefaultLine("Sent request.");

            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(response);
                Logger.DefaultLine("Received response.");
            }
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SendGroupMsgResponse>(json_string);
            return obj;
        }

        /// <summary>
        /// 发送群聊消息（异步版本）
        /// </summary>
        /// <param name="id">群号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public async Task<SendGroupMsgResponse> SendGroupMessageAsync(string groupId, string message)
        {
            string json_string = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("group_id", HttpUtility.UrlEncode(groupId));
            parameters.Add("message", HttpUtility.UrlEncode(message));

            var response = WebRequestHelper.CreatePostHttpResponseAsync(ApiUrl + "/send_group_msg_async", parameters);
            Logger.DefaultLine("Sent request.");
            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(await response);
                Logger.DefaultLine("Received response.");
            }
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SendGroupMsgResponse>(json_string);
            return obj;
        }

        public string SetGroupBan(string groupId, string userId, int duration)
        {
            string json_string = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("group_id", HttpUtility.UrlEncode(groupId));
            parameters.Add("user_id", HttpUtility.UrlEncode(userId));
            parameters.Add("duration", duration.ToString());

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/set_group_ban", parameters);
            Logger.DefaultLine("Sent request.");
            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(response);
                Logger.DefaultLine("Received response.");
            }
            return json_string;
        }
        public GroupListInfo GetGroupList()
        {
            string json_string = null;

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/get_group_list");
            //Logger.DefaultLine("Sent request.");
            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(response);
                //Logger.DefaultLine("Received response.");
            }
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<GroupListInfo>(json_string);
            return obj;
        }

        public GroupMemberInfo GetGroupMemberInfo(string groupId, string userId, bool noCache = false)
        {
            string json_string = null;

            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("group_id", HttpUtility.UrlEncode(groupId));
            parameters.Add("user_id", HttpUtility.UrlEncode(userId));
            parameters.Add("no_cache", noCache.ToString());

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/get_group_member_info", parameters);
            //Logger.DefaultLine("Sent request.");
            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(response);
                //Logger.DefaultLine("Received response.");
            }
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<GroupMemberInfo>(json_string);
            return obj;
        }

        /// <summary>
        /// 拓展方法，API没有，每次都要重新查询
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public GroupInfo GetGroupInfo(string groupId)
        {
            var info = GetGroupList();
            return info.Data.Find(x => x.GroupId.ToString() == groupId);
        }
    }
}
