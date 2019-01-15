using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.Common.Logging;
using Daylily.CoolQ;
using Daylily.CoolQ.CoolQHttp;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Linq;

namespace Daylily.Plugin.Kernel
{
    public class CliNotificationApp : CoolQApplicationPlugin
    {
        public override Guid Guid => new Guid("9a71f0a2-fd2e-4d7e-abd8-681d14d0d83e");

        public override MiddlewareConfig MiddlewareConfig { get; } = new BackendConfig
        {
            Priority = 99,
            CanDisabled = false
        };

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            long groupId = Convert.ToInt64(routeMsg.GroupId);
            long userId = Convert.ToInt64(routeMsg.UserId);
            long discussId = Convert.ToInt64(routeMsg.DiscussId);
            var type = routeMsg.MessageType;

            string group, sender, message = routeMsg.Message.RawMessage;
            var data = CoolQDispatcher.Current.SessionList[(CoolQIdentity)routeMsg.Identity].GetDataAsync().Result;

            if (type == MessageType.Private)
            {
                group = "私聊";
                sender = data.Name;
            }
            else if (type == MessageType.Discuss)
            {
                group = data.Name;
                sender = routeMsg.UserId;
            }
            else
            {
                var userInfo = data?.GroupInfo?.Members
                        ?.FirstOrDefault(i => i.UserId == userId) ??
                    CoolQHttpApiClient.GetGroupMemberInfo(routeMsg.GroupId, routeMsg.UserId).Data;
                group = data?.Name;
                sender = string.IsNullOrEmpty(userInfo.Card)
                    ? userInfo.Nickname
                    : userInfo.Card;
            }

            Logger.Message($"({group}) {sender}:\r\n  {CoolQCode.DecodeToString(message).Replace("\n", "\n  ")}");
            return null;
        }
    }
}
