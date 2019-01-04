using Daylily.Bot.Backend;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System.Collections.Concurrent;
using Daylily.Bot;

namespace Daylily.Plugin.Kernel
{
    public class CommandCounter : CoolQApplicationPlugin
    {
        public ConcurrentDictionary<string, int> CommandRate { get; private set; }

        public override MiddlewareConfig MiddlewareConfig { get; } = new BackendConfig
        {
            Priority = -3
        };

        public override void OnInitialized(string[] args)
        {
            base.OnInitialized(args);
            CommandRate = LoadSettings<ConcurrentDictionary<string, int>>("CommandRate") ??
                         new ConcurrentDictionary<string, int>();

        }

        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMessageObj)
        {
            if (string.IsNullOrEmpty(routeMessageObj.FullCommand))
                return null;
            if (!CommandRate.Keys.Contains(routeMessageObj.Command))
            {
                CommandRate.TryAdd(routeMessageObj.Command, 1);
            }
            else
            {
                CommandRate[routeMessageObj.Command]++;
            }

            SaveSettings(CommandRate, "CommandRate");
            return null;
        }
    }
}
