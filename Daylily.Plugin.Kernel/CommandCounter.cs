using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

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

        public override CommonMessageResponse OnMessageReceived(CommonMessage messageObj)
        {
            if (string.IsNullOrEmpty(messageObj.FullCommand))
                return null;
            if (!CommandRate.Keys.Contains(messageObj.Command))
            {
                CommandRate.TryAdd(messageObj.Command, 1);
            }
            else
            {
                CommandRate[messageObj.Command]++;
            }

            SaveSettings(CommandRate, "CommandRate");
            return null;
        }
    }
}
