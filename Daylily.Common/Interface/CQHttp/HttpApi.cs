using Daylily.Common.Assist;
using Daylily.Common.Models.CQResponse.Api;
using System.Collections.Generic;
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
            string jsonString = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "user_id", HttpUtility.UrlEncode(id) },
                { "message", HttpUtility.UrlEncode(message) }
            };

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/send_private_msg", parameters);
            Logger.DefaultLine("Sent request.");

            if (response != null)
            {
                jsonString = WebRequestHelper.GetResponseString(response);
                Logger.DefaultLine("Received response.");
            }
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SendPrivateMsgResponse>(jsonString);
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
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "user_id", HttpUtility.UrlEncode(id) },
                { "message", HttpUtility.UrlEncode(message) }
            };

            var response = WebRequestHelper.CreatePostHttpResponseAsync(ApiUrl + "/send_private_msg_async", parameters);
            Logger.DefaultLine("Sent request.");

            string jsonString = WebRequestHelper.GetResponseString(await response);
            Logger.DefaultLine("Received response.");

            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SendPrivateMsgResponse>(jsonString);
            return obj;
        }

        /// <summary>
        /// 发送讨论组消息
        /// </summary>
        /// <param name="discussId">讨论组号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public SendDiscussMsgResponse SendDiscussMessage(string discussId, string message)
        {
            string jsonString = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "discuss_id", HttpUtility.UrlEncode(discussId) },
                { "message", HttpUtility.UrlEncode(message) }
            };

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/send_discuss_msg", parameters);
            Logger.DefaultLine("Sent request.");

            if (response != null)
            {
                jsonString = WebRequestHelper.GetResponseString(response);
                Logger.DefaultLine("Received response.");
            }
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SendDiscussMsgResponse>(jsonString);
            return obj;
        }
        /// <summary>
        /// 发送讨论组消息（异步版本）
        /// </summary>
        /// <param name="discussId">讨论组号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public async Task<SendDiscussMsgResponse> SendDiscussMessageAsync(string discussId, string message)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "discuss_id", HttpUtility.UrlEncode(discussId) },
                { "message", HttpUtility.UrlEncode(message) }
            };

            var response = WebRequestHelper.CreatePostHttpResponseAsync(ApiUrl + "/send_discuss_msg_async", parameters);
            Logger.DefaultLine("Sent request.");

            var jsonString = WebRequestHelper.GetResponseString(await response);
            Logger.DefaultLine("Received response.");

            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SendDiscussMsgResponse>(jsonString);
            return obj;
        }

        /// <summary>
        /// 发送群聊消息
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public SendGroupMsgResponse SendGroupMessage(string groupId, string message)
        {
            string jsonString = null;
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "group_id", HttpUtility.UrlEncode(groupId) },
                { "message", HttpUtility.UrlEncode(message) }
            };

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/send_group_msg", parameters);
            Logger.DefaultLine("Sent request.");

            if (response != null)
            {
                jsonString = WebRequestHelper.GetResponseString(response);
                Logger.DefaultLine("Received response.");
            }
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SendGroupMsgResponse>(jsonString);
            return obj;
        }
        /// <summary>
        /// 发送群聊消息（异步版本）
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public async Task<SendGroupMsgResponse> SendGroupMessageAsync(string groupId, string message)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "group_id", HttpUtility.UrlEncode(groupId) },
                { "message", HttpUtility.UrlEncode(message) }
            };

            var response = WebRequestHelper.CreatePostHttpResponseAsync(ApiUrl + "/send_group_msg_async", parameters);
            Logger.DefaultLine("Sent request.");

            var jsonString = WebRequestHelper.GetResponseString(await response);
            Logger.DefaultLine("Received response.");

            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SendGroupMsgResponse>(jsonString);
            return obj;
        }

        /// <summary>
        /// 撤回消息
        /// </summary>
        /// <param name="messageId"></param>
        public void DeleteMessage(long messageId)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "message_id", messageId.ToString() }
            };

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/delete_msg", parameters);
            Logger.DefaultLine("Sent request.");

            if (response == null) return;

            WebRequestHelper.GetResponseString(response);
            Logger.DefaultLine("Received response.");
        }

        public void SetGroupBan(string groupId, string userId, int duration)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "group_id", HttpUtility.UrlEncode(groupId) },
                { "user_id", HttpUtility.UrlEncode(userId) },
                { "duration", duration.ToString() }
            };

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/set_group_ban", parameters);
            Logger.DefaultLine("Sent request.");

            if (response == null) return;

            WebRequestHelper.GetResponseString(response);
            Logger.DefaultLine("Received response.");

        }

        public GroupListInfo GetGroupList()
        {
            string jsonString = null;

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/get_group_list");
            //Logger.DefaultLine("Sent request.");
            if (response != null)
            {
                jsonString = WebRequestHelper.GetResponseString(response);
            }
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<GroupListInfo>(jsonString);
            return obj;
        }

        public GroupMemberInfo GetGroupMemberInfo(string groupId, string userId, bool noCache = false)
        {
            string jsonString = null;

            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "group_id", HttpUtility.UrlEncode(groupId) },
                { "user_id", HttpUtility.UrlEncode(userId) },
                { "no_cache", noCache.ToString() }
            };

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/get_group_member_info", parameters);
            //Logger.DefaultLine("Sent request.");
            if (response != null)
            {
                jsonString = WebRequestHelper.GetResponseString(response);
                //Logger.DefaultLine("Received response.");
            }
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<GroupMemberInfo>(jsonString);
            return obj;
        }

        public GroupMemberList GetGroupMemberList(string groupId)
        {
            string jsonString = null;

            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "group_id", HttpUtility.UrlEncode(groupId) }
            };

            var response = WebRequestHelper.CreatePostHttpResponse(ApiUrl + "/get_group_member_list", parameters);
            //Logger.DefaultLine("Sent request.");
            if (response != null)
            {
                jsonString = WebRequestHelper.GetResponseString(response);
                //Logger.DefaultLine("Received response.");
            }
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<GroupMemberList>(jsonString);
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
