using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Common.Logging;
using Daylily.CoolQ;
using Daylily.CoolQ.Messaging;
using System;
using System.Linq;
using Daylily.Bot.Messaging;
using Daylily.CoolQ.Plugin;

namespace Daylily.Plugin.Kernel
{
    public class PermissionCheckerApp : CoolQApplicationPlugin
    {
        public override Guid Guid => new Guid("9a4e9dba-f96b-4846-988d-55d2650a6f2b");

        public override bool RunInMultiThreading => false;

        public override MiddlewareConfig MiddlewareConfig { get; } = new BackendConfig
        {
            Priority = -1,
            CanDisabled = false
        };

        public static Authority? GetRequestAuthority(string message, out string fullCommand)
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

        public static Authority GetActualAuthority(CoolQRouteMessage routeMsg, out string fullCommand)
        {
            string message = routeMsg.Message.RawMessage;
            var requestAuth = GetRequestAuthority(message, out fullCommand) ?? Authority.Public;
            long userId = Convert.ToInt64(routeMsg.UserId);
            switch (requestAuth)
            {
                case Authority.Public:
                    {
                        var data = CoolQDispatcher.Current.SessionList[(CoolQIdentity)routeMsg.Identity].GetDataAsync().Result;
                        if (data.GroupInfo?.Admins.Count(q => q.UserId == userId) != 0)
                            requestAuth = Authority.Admin;
                        if (userId == 2241521134)
                            requestAuth = Authority.Root;
                        break;
                    }
                case Authority.Admin:
                    {
                        var data = CoolQDispatcher.Current.SessionList[(CoolQIdentity)routeMsg.Identity].GetDataAsync().Result;
                        if (data.GroupInfo?.Admins.Count(q => q.UserId == userId) != 0)
                            return Authority.Admin;

                        break;
                    }
                case Authority.Root:
                    if (userId == 2241521134)
                        return Authority.Root;

                    break;
            }

            return requestAuth;
        }

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            long userId = Convert.ToInt64(routeMsg.UserId);
            string message = routeMsg.Message.RawMessage;

            var requestAuth = GetRequestAuthority(message, out var fullCommand);
            switch (requestAuth)
            {
                case Authority.Public:
                    {
                        var data = CoolQDispatcher.Current.SessionList[(CoolQIdentity)routeMsg.Identity].GetDataAsync().Result;
                        if (data.GroupInfo?.Admins.Count(q => q.UserId == userId) != 0)
                            requestAuth = Authority.Admin;
                        if (userId == 2241521134)
                            requestAuth = Authority.Root;
                    }
                    break;
                case Authority.Admin:
                    {
                        var data = CoolQDispatcher.Current.SessionList[(CoolQIdentity)routeMsg.Identity].GetDataAsync().Result;
                        if (data.GroupInfo?.Admins.Count(q => q.UserId == userId) == 0)
                        {
                            Logger.Raw("Access denied.");
                            return routeMsg.ToSource(DefaultReply.FakeAdmin).Handle();
                        }
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
