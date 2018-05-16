using Daylily.Common.Assist;
using Daylily.Common.Models.CQRequest;
using Daylily.Common.Models.CQResponse;
using Daylily.Common.Models.CQResponse.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Daylily.Common.Interface.CQHttp
{
    public class HttpApi
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
        /// 发送讨论组消息
        /// </summary>
        /// <param name="id">讨论组号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public SendDiscussMsgResponse SendDiscussMessage(string discussId, string message)
        {
            string json_string = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("discuss_id", HttpUtility.UrlEncode(discussId));
            parameters.Add("message", HttpUtility.UrlEncode(message));

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/send_discuss_msg", parameters);
            Logger.DefaultLine("Sent request.");

            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(response);
                Logger.DefaultLine("Received response.");
            }
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SendDiscussMsgResponse>(json_string);
            return obj;
        }
        /// <summary>
        /// 发送讨论组消息（异步版本）
        /// </summary>
        /// <param name="id">讨论组号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public async Task<SendDiscussMsgResponse> SendDiscussMessageAsync(string discussId, string message)
        {
            string json_string = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("discuss_id", HttpUtility.UrlEncode(discussId));
            parameters.Add("message", HttpUtility.UrlEncode(message));

            var response = WebRequestHelper.CreatePostHttpResponseAsync(ApiUrl + "/send_discuss_msg_async", parameters);
            Logger.DefaultLine("Sent request.");
            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(await response);
                Logger.DefaultLine("Received response.");
            }
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SendDiscussMsgResponse>(json_string);
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

        /// <summary>
        /// 撤回消息
        /// </summary>
        /// <param name="messageId"></param>
        public void DeleteMessage(long messageId)
        {
            string json_string = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("message_id", messageId.ToString());

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/delete_msg", parameters);
            Logger.DefaultLine("Sent request.");

            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(response);
                Logger.DefaultLine("Received response.");
            }
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

        public GroupMemberList GetGroupMemberList(string groupId)
        {
            string json_string = null;

            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("group_id", HttpUtility.UrlEncode(groupId));

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/get_group_member_list", parameters);
            //Logger.DefaultLine("Sent request.");
            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(response);
                //Logger.DefaultLine("Received response.");
            }
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<GroupMemberList>(json_string);
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
