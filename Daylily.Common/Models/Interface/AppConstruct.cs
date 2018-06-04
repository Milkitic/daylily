using System;
using Daylily.Common.Assist;
using Daylily.Common.Interface.CQHttp;
using Daylily.Common.Models.CQResponse.Api;
using Daylily.Common.Models.Enum;

namespace Daylily.Common.Models.Interface
{
    public abstract class AppConstruct
    {
        protected PermissionLevel CurrentLevel { get; set; }

        protected static readonly Random Rnd = new Random();

        /// <summary>
        /// 以逗号分隔 如"sleep,slip"
        /// </summary>
        public abstract string Name { get; }
        public abstract string Author { get; }
        public abstract PluginVersion Version { get; }
        public abstract string VersionNumber { get; }
        public abstract string Description { get; }
        public abstract string Command { get; }
        public abstract AppType AppType { get; }

        public abstract void OnLoad(string[] args);
        public abstract CommonMessageResponse OnExecute(CommonMessage commonMessage);

        public static void SendMessage(CommonMessageResponse response)
        {
            switch (response.MessageType)
            {
                case MessageType.Group:
                    SendGroupMsgResponse groupMsg = CqApi.SendGroupMessageAsync(response.GroupId,
                        (response.EnableAt ? CqCode.EncodeAt(response.UserId) + " " : "") + response.Message);
                    Logger.InfoLine($"我: {CqCode.Decode(response.Message)} {{status: {groupMsg.Status}}})");
                    break;
                case MessageType.Discuss:
                    SendDiscussMsgResponse discussMsg = CqApi.SendDiscussMessageAsync(response.DiscussId,
                        (response.EnableAt ? CqCode.EncodeAt(response.UserId) + " " : "") + response.Message);
                    Logger.InfoLine($"我: {CqCode.Decode(response.Message)} {{status: {discussMsg.Status}}})");
                    break;
                case MessageType.Private:
                    SendPrivateMsgResponse privateMsg =
                        CqApi.SendPrivateMessageAsync(response.UserId, response.Message);
                    Logger.InfoLine($"我: {CqCode.Decode(response.Message)} {{status: {privateMsg.Status}}})");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void SendMessage(CommonMessageResponse response, string groupId, string discussId,
            MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Group:
                    SendGroupMsgResponse groupMsg = CqApi.SendGroupMessageAsync(groupId,
                        (response.EnableAt ? CqCode.EncodeAt(response.UserId) + " " : "") + response.Message);
                    Logger.InfoLine($"我: {CqCode.Decode(response.Message)} {{status: {groupMsg.Status}}})");
                    break;
                case MessageType.Discuss:
                    SendDiscussMsgResponse discussMsg = CqApi.SendDiscussMessageAsync(discussId.ToString(),
                        (response.EnableAt ? CqCode.EncodeAt(response.UserId) + " " : "") + response.Message);
                    Logger.InfoLine($"我: {CqCode.Decode(response.Message)} {{status: {discussMsg.Status}}})");
                    break;
                case MessageType.Private:
                    SendPrivateMsgResponse privateMsg =
                        CqApi.SendPrivateMessageAsync(response.UserId, response.Message);
                    Logger.InfoLine($"我: {CqCode.Decode(response.Message)} {{status: {privateMsg.Status}}})");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
