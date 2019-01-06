using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Daylily.Plugin.Kernel
{
    public class PluginSwitch : CoolQApplicationPlugin
    {
        public override Guid Guid => new Guid("20e7a3e1-fdc3-4b3a-bff1-ecf396a63311");

        private static ConcurrentDictionary<CoolQIdentity, List<Guid>> DisabledList => PluginManager.
            DisabledList;

        public override bool RunInMultiThreading => false;

        public override MiddlewareConfig MiddlewareConfig { get; } = new BackendConfig
        {
            CanDisabled = false,
            Priority = 88
        };

        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMsg)
        {
            if (DisabledList.Keys.Contains((CoolQIdentity)routeMsg.Identity))
            {
                var guidList = DisabledList[(CoolQIdentity)routeMsg.Identity];
                PermissionChecker.GetAuthority(routeMsg.RawMessage, out var fullCommand);
                var cmdName = fullCommand?.Split(' ')[0];
                if (cmdName != null)
                {
                    Guid? pluginGuid = Bot.Backend.PluginManager.Current.GetPlugin(cmdName)?.Guid;
                    if (pluginGuid != null)
                    {
                        if (guidList.Contains(pluginGuid.Value))
                            return routeMsg.ToSource("本群已禁用此命令.").Handle();
                    }
                }
                else
                {
                    throw new NotImplementedException();
                    //Guid? pluginGuid = null;
                    //if (pluginGuid != null)
                    //{
                    //    return new CoolQRouteMessage().Handle();
                    //}
                }
            }

            return null;
        }
    }
}
