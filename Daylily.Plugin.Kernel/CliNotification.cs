using System;
using System.Linq;
using Daylily.Bot;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ;
using Daylily.CoolQ.Interface.CqHttp;

namespace Daylily.Plugin.Kernel
{
    public class CliNotification : ApplicationPlugin
    {
        public override BackendConfig BackendConfig { get; } = new BackendConfig
        {
            Priority = 99
        };

        public override CommonMessageResponse OnMessageReceived(CommonMessage messageObj)
        {
            var cm = messageObj;
            long groupId = Convert.ToInt64(cm.GroupId);
            long userId = Convert.ToInt64(cm.UserId);
            long discussId = Convert.ToInt64(cm.DiscussId);
            var type = cm.MessageType;

            string group, sender, message = cm.RawMessage;
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

            Logger.Message($"({group}) {sender}:\r\n  {CqCode.DecodeToString(message)}");
            return null;
        }
    }
}
