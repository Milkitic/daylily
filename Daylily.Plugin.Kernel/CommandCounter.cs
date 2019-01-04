using Daylily.Bot.Message;
using System.Collections.Concurrent;
using Daylily.Bot.Backend;
using Daylily.CoolQ.Message;

namespace Daylily.Plugin.Kernel
{
    public class CommandCounter : ApplicationPlugin
    {
        public ConcurrentDictionary<string, int> CommandRate { get; private set; }

        public override BackendConfig BackendConfig { get; } = new BackendConfig
        {
            Priority = -3
        };

        public override void OnInitialized(string[] args)
        {
            base.OnInitialized(args);
            CommandRate = LoadSettings<ConcurrentDictionary<string, int>>("CommandRate") ??
                         new ConcurrentDictionary<string, int>();

        }

        public override CommonMessageResponse OnMessageReceived(CoolQNavigableMessage navigableMessageObj)
        {
            if (string.IsNullOrEmpty(navigableMessageObj.FullCommand))
                return null;
            if (!CommandRate.Keys.Contains(navigableMessageObj.Command))
            {
                CommandRate.TryAdd(navigableMessageObj.Command, 1);
            }
            else
            {
                CommandRate[navigableMessageObj.Command]++;
            }

            SaveSettings(CommandRate, "CommandRate");
            return null;
        }
    }
}
