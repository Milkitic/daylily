using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Linq;
using Daylily.CoolQ.CoolQHttp;

namespace Daylily.Plugin.Kernel
{
    public class CliNotification : CoolQApplicationPlugin
    {
        public override Guid Guid => new Guid("9a71f0a2-fd2e-4d7e-abd8-681d14d0d83e");

        public override MiddlewareConfig MiddlewareConfig { get; } = new BackendConfig
        {
            Priority = 99
        };

        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMessageObj)
        {
            var cm = routeMessageObj;
            long groupId = Convert.ToInt64(cm.GroupId);
            long userId = Convert.ToInt64(cm.UserId);
            long discussId = Convert.ToInt64(cm.DiscussId);
            var type = cm.MessageType;

            string group, sender, message = cm.Message.RawMessage;
            if (type == MessageType.Private)
            {
                group = "私聊";
                sender = CoolQDispatcher.Current.SessionInfo[(CoolQIdentity)cm.Identity].Name;
            }
            else if (type == MessageType.Discuss)
            {
                group = CoolQDispatcher.Current.SessionInfo[(CoolQIdentity)cm.Identity].Name;
                sender = cm.UserId;
            }
            else
            {
                var userInfo =
                    CoolQDispatcher.Current.SessionInfo[(CoolQIdentity) cm.Identity]?.GroupInfo?.Members
                        ?.FirstOrDefault(i => i.UserId == userId) ??
                    CoolQHttpApi.GetGroupMemberInfo(cm.GroupId, cm.UserId).Data;
                group = CoolQDispatcher.Current.SessionInfo?[(CoolQIdentity)cm.Identity]?.Name;
                sender = string.IsNullOrEmpty(userInfo.Card)
                    ? userInfo.Nickname
                    : userInfo.Card;
            }

            Logger.Message($"({group}) {sender}:\r\n  {CoolQCode.DecodeToString(message)}");
            return null;
        }
    }
}
