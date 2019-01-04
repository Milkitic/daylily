using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Enum;
using Daylily.Bot.Message;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Linq;

namespace Daylily.Plugin.Kernel
{
    public class PermissionChecker : CoolQApplicationPlugin
    {
        public override bool RunInMultiThreading => false;

        public override BackendConfig BackendConfig { get; } = new BackendConfig
        {
            Priority = -1
        };

        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMsg)
        {
            var cm = routeMsg;

            long groupId = Convert.ToInt64(cm.GroupId);
            long userId = Convert.ToInt64(cm.UserId);
            long discussId = Convert.ToInt64(cm.DiscussId);
            string message = cm.Message.RawMessage;
            var type = cm.MessageType;

            if (message.Substring(0, 1) == Bot.Core.Current.CommandFlag)
            {
                if (message.IndexOf(Bot.Core.Current.CommandFlag + "root ", StringComparison.InvariantCulture) == 0)
                {
                    if (cm.UserId != "2241521134")
                    {
                        Logger.Raw("Access denied.");
                        return routeMsg.ToSource(LoliReply.FakeRoot).Handle();
                    }
                    else
                    {
                        cm.FullCommand = message.Substring(6, message.Length - 6);
                        cm.Authority = Authority.Root;
                    }

                }
                else if (message.IndexOf(Bot.Core.Current.CommandFlag + "sudo ", StringComparison.InvariantCulture) == 0 &&
                         cm.MessageType == MessageType.Group)
                {
                    if (CoolQDispatcher.Current.SessionInfo[(CqIdentity)cm.Identity].GroupInfo.Admins.Count(q => q.UserId == userId) == 0)
                    {
                        Logger.Raw("Access denied.");
                        return routeMsg.ToSource(LoliReply.FakeAdmin).Handle();
                    }
                    else
                    {
                        cm.FullCommand = message.Substring(6, message.Length - 6);
                        cm.Authority = Authority.Admin;
                    }
                }
                else
                {
                    // auto
                    if (CoolQDispatcher.Current.SessionInfo[(CqIdentity)cm.Identity].GroupInfo?.Admins.Count(q => q.UserId == userId) != 0)
                        cm.Authority = Authority.Admin;
                    if (cm.UserId == "2241521134")
                        cm.Authority = Authority.Root;

                    cm.FullCommand = message.Substring(1, message.Length - 1);
                }
            }

            return null;
        }
    }
}
