using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Command;
using Daylily.CoolQ;
using Daylily.CoolQ.Messaging;
using System;
using Daylily.CoolQ.Plugin;

namespace Daylily.Plugin.Kernel
{
    public class CommandParserApp : CoolQApplicationPlugin
    {
        public override Guid Guid => new Guid("cc509b6d-5a7c-4579-a398-1e895ac1664a");

        public override bool RunInMultiThreading => false;

        public override MiddlewareConfig MiddlewareConfig { get; } = new BackendConfig
        {
            Priority = -2,
            CanDisabled = false
        };

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            if (string.IsNullOrEmpty(routeMsg.FullCommand))
                return null;
            string fullCmd = routeMsg.FullCommand;
            var ca = new CommandAnalyzer<StreamParamDivider>();
            ca.Analyze(fullCmd, routeMsg);

            if (!DaylilyCore.Current.PluginManager.ContainsPlugin(routeMsg.CommandName))
                return null;

            return null;
        }
    }
}
