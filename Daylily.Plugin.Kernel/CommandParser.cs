using Daylily.Bot;
using Daylily.Bot.Command;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Plugin.Kernel
{
    public class CommandParser : ApplicationPlugin
    {
        public override bool RunInMultiThreading => false;

        public override BackendConfig BackendConfig { get; } = new BackendConfig
        {
            Priority = -2
        };

        public override CommonMessageResponse OnMessageReceived(CommonMessage messageObj)
        {
            if (string.IsNullOrEmpty(messageObj.FullCommand))
                return null;
            string fullCmd = messageObj.FullCommand;
            CommandAnalyzer ca = new CommandAnalyzer(new ParamDividerV2());
            ca.Analyze(fullCmd, messageObj);

            if (!PluginManager.CommandMap.ContainsKey(messageObj.Command))
                return null;

            return null;
        }
    }
}
