using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Command;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;

namespace Daylily.Plugin.Kernel
{
    public class CommandParser : CoolQApplicationPlugin
    {
        public override bool RunInMultiThreading => false;

        public override BackendConfig BackendConfig { get; } = new BackendConfig
        {
            Priority = -2
        };

        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMessageObj)
        {
            if (string.IsNullOrEmpty(routeMessageObj.FullCommand))
                return null;
            string fullCmd = routeMessageObj.FullCommand;
            CommandAnalyzer ca = new CommandAnalyzer(new ParamDividerV2());
            ca.Analyze(fullCmd, routeMessageObj);

            if (!Core.Current.PluginManager.ContainsPlugin(routeMessageObj.Command))
                return null;

            return null;
        }
    }
}
