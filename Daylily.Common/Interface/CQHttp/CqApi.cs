using Daylily.Common.Assist;
using Daylily.Common.Models.CQResponse.Api;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Daylily.Common.Interface.CQHttp
{
    public static class CqApi
    {
        public static string ApiUrl { private get; set; } = "http://www.mothership.top:5700";

        private const string PrivateMsgPath = "/send_private_msg";
        private const string PrivateMsgAsyncPath = "/send_private_msg_async";
        private const string DiscussMsgPath = "/send_discuss_msg";
        private const string DiscussMsgAsyncPath = "/send_discuss_msg_async";
        private const string GroupMsgPath = "/send_group_msg";
        private const string GroupMsgAsyncPath = "/send_group_msg_async";

        private const string MsgDelPath = "/delete_msg";
        private const string GroupBanPath = "/set_group_ban";
        private const string GroupListPath = "/get_group_list";
        private const string GroupMemberInfoPath = "/get_group_member_info";
        private const string GroupMemberListPath = "/get_group_member_list";

        /// <summary>
        /// 发送私聊消息
        /// </summary>
        /// <param name="id">对方 QQ 号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public static SendPrivateMsgResponse SendPrivateMessage(string id, string message)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "user_id", HttpUtility.UrlEncode(id) },
                { "message", HttpUtility.UrlEncode(message) }
            };
            return Request<SendPrivateMsgResponse>(ApiUrl + PrivateMsgPath, parameters, true);
        }
        /// <summary>
        /// 发送私聊消息（异步版本）
        /// </summary>
        /// <param name="id">对方 QQ 号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public static SendPrivateMsgResponse SendPrivateMessageAsync(string id, string message)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "user_id", HttpUtility.UrlEncode(id) },
                { "message", HttpUtility.UrlEncode(message) }
            };
            return AsyncRequest<SendPrivateMsgResponse>(ApiUrl + PrivateMsgAsyncPath, parameters, true).Result;
        }

        /// <summary>
        /// 发送讨论组消息
        /// </summary>
        /// <param name="discussId">讨论组号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public static SendDiscussMsgResponse SendDiscussMessage(string discussId, string message)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "discuss_id", HttpUtility.UrlEncode(discussId) },
                { "message", HttpUtility.UrlEncode(message) }
            };
            return Request<SendDiscussMsgResponse>(ApiUrl + DiscussMsgPath, parameters, true);
        }
        /// <summary>
        /// 发送讨论组消息（异步版本）
        /// </summary>
        /// <param name="discussId">讨论组号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public static SendDiscussMsgResponse SendDiscussMessageAsync(string discussId, string message)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "discuss_id", HttpUtility.UrlEncode(discussId) },
                { "message", HttpUtility.UrlEncode(message) }
            };
            return AsyncRequest<SendDiscussMsgResponse>(ApiUrl + DiscussMsgAsyncPath, parameters, true).Result;
        }

        /// <summary>
        /// 发送群聊消息
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public static SendGroupMsgResponse SendGroupMessage(string groupId, string message)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "group_id", HttpUtility.UrlEncode(groupId) },
                { "message", HttpUtility.UrlEncode(message) }
            };
            return Request<SendGroupMsgResponse>(ApiUrl + GroupMsgPath, parameters, true);
        }
        /// <summary>
        /// 发送群聊消息（异步版本）
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public static SendGroupMsgResponse SendGroupMessageAsync(string groupId, string message)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "group_id", HttpUtility.UrlEncode(groupId) },
                { "message", HttpUtility.UrlEncode(message) }
            };
            return AsyncRequest<SendGroupMsgResponse>(ApiUrl + GroupMsgAsyncPath, parameters, true).Result;
        }

        /// <summary>
        /// 撤回消息
        /// </summary>
        /// <param name="messageId"></param>
        public static void DeleteMessage(long messageId)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "message_id", messageId.ToString() }
            };
            Request(ApiUrl + MsgDelPath, parameters, true);
        }

        public static void SetGroupBan(string groupId, string userId, int duration)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "group_id", HttpUtility.UrlEncode(groupId) },
                { "user_id", HttpUtility.UrlEncode(userId) },
                { "duration", duration.ToString() }
            };
            Request(ApiUrl + GroupBanPath, parameters, true);
        }

        public static GroupListInfo GetGroupList() => Request<GroupListInfo>(ApiUrl + GroupListPath, null);

        public static GroupMemberInfo GetGroupMemberInfo(string groupId, string userId, bool noCache = false)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "group_id", HttpUtility.UrlEncode(groupId) },
                { "user_id", HttpUtility.UrlEncode(userId) },
                { "no_cache", noCache.ToString() }
            };

            return Request<GroupMemberInfo>(ApiUrl + GroupMemberInfoPath, parameters);
        }

        public static GroupMemberList GetGroupMemberList(string groupId)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "group_id", HttpUtility.UrlEncode(groupId) }
            };
            return Request<GroupMemberList>(ApiUrl + GroupMemberListPath, parameters);
        }

        /// <summary>
        /// 拓展方法，每次都要重新查询
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static GroupInfo GetGroupInfo(string groupId) => GetGroupList().Data.Find(x => x.GroupId.ToString() == groupId);

        private static void Request(string url, IDictionary<string, string> parameters, bool enableLog = false)
        {
            var response = WebRequestHelper.CreatePostHttpResponse(url, parameters);
            if (response == null)
                return;
            if (enableLog)
                Logger.DefaultLine("Sent request.");
            WebRequestHelper.GetResponseString(response);
            if (enableLog)
                Logger.DefaultLine("Received response.");
        }

        private static T Request<T>(string url, IDictionary<string, string> parameters, bool enableLog = false)
        {
            var response = WebRequestHelper.CreatePostHttpResponse(url, parameters);
            if (response == null)
                return default;
            if (enableLog)
                Logger.DefaultLine("Sent request.");
            var jsonString = WebRequestHelper.GetResponseString(response);
            if (enableLog)
                Logger.DefaultLine("Received response.");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
        }

        private static async Task<T> AsyncRequest<T>(string url, IDictionary<string, string> parameters, bool enableLog = false)
        {
            var response = WebRequestHelper.CreatePostHttpResponseAsync(url, parameters);
            if (response.Result == null)
                return default;
            if (enableLog)
                Logger.DefaultLine("Sent request.");
            var jsonString = WebRequestHelper.GetResponseString(await response);
            if (enableLog)
                Logger.DefaultLine("Received response.");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
