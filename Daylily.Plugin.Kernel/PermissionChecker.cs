using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.Common.Logging;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Linq;

namespace Daylily.Plugin.Kernel
{
    public class PermissionChecker : CoolQApplicationPlugin
    {
        public override Guid Guid => new Guid("9a4e9dba-f96b-4846-988d-55d2650a6f2b");

        public override bool RunInMultiThreading => false;

        public override MiddlewareConfig MiddlewareConfig { get; } = new BackendConfig
        {
            Priority = -1,
            CanDisabled = false
        };

        public static Authority? GetAuthority(string message, out string fullCommand)
        {
            if (message.StartsWith(DaylilyCore.Current.CommandFlag))
            {
                if (message.StartsWith($"{DaylilyCore.Current.CommandFlag}root ") ||
                    message.Trim() == $"{DaylilyCore.Current.CommandFlag}root")
                {
                    fullCommand = message.Substring(5).Trim();
                    return Authority.Root;
                }
                else if (message.StartsWith($"{DaylilyCore.Current.CommandFlag}sudo ") ||
                         message.Trim() == $"{DaylilyCore.Current.CommandFlag}sudo")
                {
                    fullCommand = message.Substring(5).Trim();
                    return Authority.Admin;
                }
                else
                {
                    fullCommand = message.Substring(1);
                    return Authority.Public;
                }
            }

            fullCommand = null;
            return null;
        }

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            long userId = Convert.ToInt64(routeMsg.UserId);
            string message = routeMsg.Message.RawMessage;

            var requestAuth = GetAuthority(message, out var fullCommand);
            switch (requestAuth)
            {
                case Authority.Public:
                    if (CoolQDispatcher.Current.SessionInfo[(CoolQIdentity)routeMsg.Identity].GroupInfo
                            ?.Admins.Count(q => q.UserId == userId) != 0)
                        requestAuth = Authority.Admin;
                    if (userId == 2241521134)
                        requestAuth = Authority.Root;
                    break;
                case Authority.Admin:
                    if (CoolQDispatcher.Current.SessionInfo[(CoolQIdentity)routeMsg.Identity].GroupInfo
                            ?.Admins.Count(q => q.UserId == userId) == 0)
                    {
                        Logger.Raw("Access denied.");
                        return routeMsg.ToSource(DefaultReply.FakeAdmin).Handle();
                    }

                    break;
                case Authority.Root:
                    if (userId != 2241521134)
                    {
                        Logger.Raw("Access denied.");
                        return routeMsg.ToSource(DefaultReply.FakeRoot).Handle();
                    }

                    break;
            }

            if (fullCommand != null)
                routeMsg.FullCommand = fullCommand;
            if (requestAuth != null)
                routeMsg.CurrentAuthority = requestAuth.Value;
            return null;
        }
    }
}
