using Daylily.Common.Assist;
using Daylily.Common.Interface.CQHttp;
using Daylily.Common.Models.CQResponse.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Common.Models
{
    public abstract class AppConstruct
    {
        protected static HttpApi CQApi = new HttpApi();
        protected PermissionLevel CurrentLevel { get; set; }

        public Random rnd = new Random();

        public abstract CommonMessageResponse Execute(CommonMessage commonMessage);

        public static void SendMessage(CommonMessageResponse response)
        {
            switch (response.MessageType)
            {
                case MessageType.Group:
                    SendGroupMsgResponse groupMsg = CQApi.SendGroupMessageAsync(response.GroupId,
                        (response.EnableAt ? Assist.CQCode.EncodeAt(response.UserId) + " " : "") + response.Message).Result;
                    Logger.InfoLine($"我: {Assist.CQCode.Decode(response.Message)} {{status: {groupMsg.Status}}})");
                    break;
                case MessageType.Discuss:
                    SendDiscussMsgResponse discussMsg = CQApi.SendDiscussMessageAsync(response.DiscussId,
                        (response.EnableAt ? Assist.CQCode.EncodeAt(response.UserId) + " " : "") + response.Message).Result;
                    Logger.InfoLine($"我: {Assist.CQCode.Decode(response.Message)} {{status: {discussMsg.Status}}})");
                    break;
                case MessageType.Private:
                    SendPrivateMsgResponse privateMsg = CQApi.SendPrivateMessageAsync(response.UserId, response.Message).Result;
                    Logger.InfoLine($"我: {Assist.CQCode.Decode(response.Message)} {{status: {privateMsg.Status}}})");
                    break;
            }
        }

        public static void SendMessage(CommonMessageResponse response, string groupId, string discussId, MessageType messageType)
        {
            switch (response.MessageType)
            {
                case MessageType.Group:
                    SendGroupMsgResponse groupMsg = CQApi.SendGroupMessageAsync(groupId.ToString(),
                        (response.EnableAt ? Assist.CQCode.EncodeAt(response.UserId) + " " : "") + response.Message).Result;
                    Logger.InfoLine($"我: {Assist.CQCode.Decode(response.Message)} {{status: {groupMsg.Status}}})");
                    break;
                case MessageType.Discuss:
                    SendDiscussMsgResponse discussMsg = CQApi.SendDiscussMessageAsync(discussId.ToString(),
                        (response.EnableAt ? Assist.CQCode.EncodeAt(response.UserId) + " " : "") + response.Message).Result;
                    Logger.InfoLine($"我: {Assist.CQCode.Decode(response.Message)} {{status: {discussMsg.Status}}})");
                    break;
                case MessageType.Private:
                    SendPrivateMsgResponse privateMsg = CQApi.SendPrivateMessageAsync(response.UserId, response.Message).Result;
                    Logger.InfoLine($"我: {Assist.CQCode.Decode(response.Message)} {{status: {privateMsg.Status}}})");
                    break;
            }
        }
    }
}
