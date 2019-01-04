using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Command;
using Daylily.Bot.Message;
using Daylily.CoolQ.Message;

namespace Daylily.Plugin.Kernel
{
    public class CommandParser : ApplicationPlugin
    {
        public override bool RunInMultiThreading => false;

        public override BackendConfig BackendConfig { get; } = new BackendConfig
        {
            Priority = -2
        };

        public override CommonMessageResponse OnMessageReceived(CoolQNavigableMessage navigableMessageObj)
        {
            if (string.IsNullOrEmpty(navigableMessageObj.FullCommand))
                return null;
            string fullCmd = navigableMessageObj.FullCommand;
            CommandAnalyzer ca = new CommandAnalyzer(new ParamDividerV2());
            ca.Analyze(fullCmd, navigableMessageObj);

            if (!PluginManager.CommandMap.ContainsKey(navigableMessageObj.Command))
                return null;

            return null;
        }
    }
}
