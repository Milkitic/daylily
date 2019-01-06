using System;
using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Command;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;

namespace Daylily.Plugin.Kernel
{
    public class CommandParser : CoolQApplicationPlugin
    {
        public override Guid Guid => new Guid("cc509b6d-5a7c-4579-a398-1e895ac1664a");

        public override bool RunInMultiThreading => false;

        public override MiddlewareConfig MiddlewareConfig { get; } = new BackendConfig
        {
            Priority = -2,
            CanDisabled = false
        };

        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMessageObj)
        {
            if (string.IsNullOrEmpty(routeMessageObj.FullCommand))
                return null;
            string fullCmd = routeMessageObj.FullCommand;
            CommandAnalyzer ca = new CommandAnalyzer(new ParamDividerV2());
            ca.Analyze(fullCmd, routeMessageObj);

            if (!DaylilyCore.Current.PluginManager.ContainsPlugin(routeMessageObj.Command))
                return null;

            return null;
        }
    }
}
