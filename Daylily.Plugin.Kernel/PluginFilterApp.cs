using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.CoolQ;
using Daylily.CoolQ.Messaging;
using System;
using System.Collections.Generic;
using Daylily.Bot.Messaging;
using Daylily.CoolQ.Plugin;

namespace Daylily.Plugin.Kernel
{
    public class PluginFilterApp : CoolQApplicationPlugin
    {
        public override Guid Guid => new Guid("20e7a3e1-fdc3-4b3a-bff1-ecf396a63311");

        private static CoolQIdentityDictionary<List<Guid>> DisabledList => PluginSwitcher.
            DisabledList;

        public override bool RunInMultiThreading => false;

        public override MiddlewareConfig MiddlewareConfig { get; } = new BackendConfig
        {
            CanDisabled = false,
            Priority = 88
        };

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            if (DisabledList.Keys.Contains((CoolQIdentity)routeMsg.Identity))
            {
                var guidList = DisabledList[(CoolQIdentity)routeMsg.Identity];
                PermissionCheckerApp.GetRequestAuthority(routeMsg.RawMessage, out var fullCommand);
                var cmdName = fullCommand?.Split(' ')[0];
                foreach (var plugin in Bot.Backend.PluginManager.Current.ApplicationInstances)
                {
                    if (!guidList.Contains(plugin.Guid)) continue;
                    scope.DisabledApplications.Add(plugin);
                }

                if (cmdName == null) return null;
                Guid? pluginGuid = Bot.Backend.PluginManager.Current.GetPlugin(cmdName)?.Guid;
                if (pluginGuid == null) return null;
                if (guidList.Contains(pluginGuid.Value))
                {
                    return routeMsg.MessageType == MessageType.Private
                        ? routeMsg.ToSource("你已禁用此命令.").Handle()
                        : routeMsg.ToSource("本群已禁用此命令.").Handle();
                }
            }

            return null;
        }
    }
}
