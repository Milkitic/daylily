using Daylily.Bot;
using Daylily.Bot.Message;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ.Interface.CqHttp;
using System;
using System.Linq;
using Daylily.Bot.Backend;
using Daylily.CoolQ.Message;

namespace Daylily.Plugin.Kernel
{
    public class CliNotification : ApplicationPlugin
    {
        public override BackendConfig BackendConfig { get; } = new BackendConfig
        {
            Priority = 99
        };

        public override CommonMessageResponse OnMessageReceived(CoolQNavigableMessage navigableMessageObj)
        {
            var cm = navigableMessageObj;
            long groupId = Convert.ToInt64(cm.GroupId);
            long userId = Convert.ToInt64(cm.UserId);
            long discussId = Convert.ToInt64(cm.DiscussId);
            var type = cm.MessageType;

            string group, sender, message = cm.Message.RawMessage;
            if (type == MessageType.Private)
            {
                group = "私聊";
                sender = CoolQDispatcher.Current.SessionInfo[cm.CqIdentity].Name;
            }
            else if (type == MessageType.Discuss)
            {
                group = CoolQDispatcher.Current.SessionInfo[cm.CqIdentity].Name;
                sender = cm.UserId;
            }
            else
            {
                var userInfo =
                    CoolQDispatcher.Current.SessionInfo[cm.CqIdentity]?.GroupInfo?.Members
                        ?.FirstOrDefault(i => i.UserId == userId) ??
                    CqApi.GetGroupMemberInfo(cm.GroupId, cm.UserId).Data;
                group = CoolQDispatcher.Current.SessionInfo?[cm.CqIdentity]?.Name;
                sender = string.IsNullOrEmpty(userInfo.Card)
                    ? userInfo.Nickname
                    : userInfo.Card;
            }

            Logger.Message($"({group}) {sender}:\r\n  {CoolQCode.DecodeToString(message)}");
            return null;
        }
    }
}
