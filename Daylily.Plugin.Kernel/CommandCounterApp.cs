using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.CoolQ;
using Daylily.CoolQ.Messaging;
using System;
using System.Collections.Concurrent;
using Daylily.CoolQ.Plugin;

namespace Daylily.Plugin.Kernel
{
    public class CommandCounterApp : CoolQApplicationPlugin
    {
        public override Guid Guid => new Guid("fe577f01-b63f-45e2-88bd-3236224b93b9");

        public ConcurrentDictionary<string, int> CommandRate { get; private set; }

        public override MiddlewareConfig MiddlewareConfig { get; } = new BackendConfig
        {
            Priority = 8,
            CanDisabled = false
        };

        public override void OnInitialized(StartupConfig startup)
        {
            base.OnInitialized(startup);
            CommandRate = LoadSettings<ConcurrentDictionary<string, int>>("CommandRate") ??
                         new ConcurrentDictionary<string, int>();
        }

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            if (string.IsNullOrEmpty(routeMsg.FullCommand))
                return null;
            if (!CommandRate.Keys.Contains(routeMsg.CommandName))
            {
                CommandRate.TryAdd(routeMsg.CommandName, 1);
            }
            else
            {
                CommandRate[routeMsg.CommandName]++;
            }

            SaveSettings(CommandRate, "CommandRate");
            return null;
        }
    }
}
