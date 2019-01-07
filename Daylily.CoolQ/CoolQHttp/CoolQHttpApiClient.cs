using Daylily.Common.Logging;
using Daylily.Common.Web;
using Daylily.CoolQ.CoolQHttp.ResponseModel;
using Daylily.CoolQ.CoolQHttp.ResponseModel.Abstract;
using System;
using System.Collections.Generic;

namespace Daylily.CoolQ.CoolQHttp
{
    public static class CoolQHttpApiClient
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

        private const string GroupInfoPath = "/_get_group_info";
        private const string StrangerInfoPath = "/get_stranger_info";

        /// <summary>
        /// 发送私聊消息
        /// </summary>
        /// <param name="id">对方 QQ 号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public static SendPrivateMsgResp SendPrivateMessage(string id, string message)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"user_id", id},
                {"message", message}
            };
            return Request<SendPrivateMsgResp>(ApiUrl + PrivateMsgPath, parameters);
        }

        /// <summary>
        /// 发送私聊消息（异步版本）
        /// </summary>
        /// <param name="id">对方 QQ 号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public static SendPrivateMsgResp SendPrivateMessageAsync(string id, string message)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"user_id", id},
                {"message", message}
            };
            return Request<SendPrivateMsgResp>(ApiUrl + PrivateMsgAsyncPath, parameters);
        }

        /// <summary>
        /// 发送讨论组消息
        /// </summary>
        /// <param name="discussId">讨论组号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public static SendDiscussMsgResp SendDiscussMessage(string discussId, string message)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"discuss_id", discussId},
                {"message", message}
            };
            return Request<SendDiscussMsgResp>(ApiUrl + DiscussMsgPath, parameters);
        }

        /// <summary>
        /// 发送讨论组消息（异步版本）
        /// </summary>
        /// <param name="discussId">讨论组号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public static SendDiscussMsgResp SendDiscussMessageAsync(string discussId, string message)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"discuss_id", discussId},
                {"message", message}
            };
            return Request<SendDiscussMsgResp>(ApiUrl + DiscussMsgAsyncPath, parameters);
        }

        /// <summary>
        /// 发送群聊消息
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public static SendGroupMsgResp SendGroupMessage(string groupId, string message)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"group_id", groupId},
                {"message", message}
            };
            return Request<SendGroupMsgResp>(ApiUrl + GroupMsgPath, parameters);
        }

        /// <summary>
        /// 发送群聊消息（异步版本）
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <param name="message">要发送的内容</param>
        /// <returns></returns>
        public static SendGroupMsgResp SendGroupMessageAsync(string groupId, string message)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"group_id", groupId},
                {"message", message}
            };
            return Request<SendGroupMsgResp>(ApiUrl + GroupMsgAsyncPath, parameters);
        }

        public static GetStrangerInfo GetStrangerInfo(string userId, bool noCache = false)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"user_id", userId},
                {"no_cache", noCache.ToString()}
            };
            return Request<GetStrangerInfo>(ApiUrl + StrangerInfoPath, parameters);
        }

        /// <summary>
        /// 撤回消息
        /// </summary>
        /// <param name="messageId"></param>
        public static void DeleteMessage(long messageId)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"message_id", messageId.ToString()}
            };
            Request(ApiUrl + MsgDelPath, parameters);
        }

        public static void SetGroupBan(string groupId, string userId, int duration)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"group_id", groupId},
                {"user_id", userId},
                {"duration", duration.ToString()}
            };
            Request(ApiUrl + GroupBanPath, parameters);
        }

        public static GetGroupList GetGroupList() => Request<GetGroupList>(ApiUrl + GroupListPath, null);

        public static GetGroupMemberInfo GetGroupMemberInfo(string groupId, string userId, bool noCache = false)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"group_id", groupId},
                {"user_id", userId},
                {"no_cache", noCache.ToString()}
            };

            return Request<GetGroupMemberInfo>(ApiUrl + GroupMemberInfoPath, parameters);
        }

        public static GetGroupMemberList GetGroupMemberList(string groupId)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"group_id", groupId}
            };
            return Request<GetGroupMemberList>(ApiUrl + GroupMemberListPath, parameters);
        }

        /// <summary>
        /// 当HTTP API版本高于4.0时，请使用此方法
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static GetGroupInfo GetGroupInfoV2(string groupId)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"group_id", groupId}
            };
            return Request<GetGroupInfo>(ApiUrl + GroupInfoPath, parameters);
        }

        /// <summary>
        /// 拓展方法，需每次重新查询
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [Obsolete("当HTTP API版本高于4.0时，此方法过时。")]
        public static GroupInfo GetGroupInfo(string groupId) =>
            GetGroupList().Data.Find(x => x.GroupId.ToString() == groupId);

        private static void Request(string url, IDictionary<string, string> parameters, bool enableLog = false)
        {
            HttpClient.HttpPost(url, parameters);
        }

        private static T Request<T>(string url, IDictionary<string, string> parameters, bool enableLog = false)
        {
            var jsonString = HttpClient.HttpPost(url, parameters);
            if (jsonString == null)
                Logger.Error("请求HTTP API失败，请检查连接是否正常");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
