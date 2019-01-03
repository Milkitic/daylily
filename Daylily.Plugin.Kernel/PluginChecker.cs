using Daylily.Bot;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daylily.Plugin.Kernel
{
    [Name("插件检查")]
    [Author("yf_extension")]
    [Version(0, 1, 0, PluginVersion.Stable)]
    [Help("检查插件的情况。", Authority = Authority.Root)]
    [Command("check")]
    public class PluginChecker : CommandPlugin
    {
        public override CommonMessageResponse OnMessageReceived(CommonMessage messageObj)
        {
            var grouped = PluginManager.ApplicationList
                .OrderByDescending(k => k.BackendConfig?.Priority)
                .GroupBy(k => k.BackendConfig?.Priority);
            StringBuilder sb = new StringBuilder();
            foreach (var plugins in grouped)
            {
                sb.Append(plugins.Key + ": ");
                foreach (var plugin in plugins)
                {
                    sb.Append(plugin.Name + (plugin.RunInMultiThreading ? "" : " (BLOCK)") + " → ");
                }

                sb.Remove(sb.Length - 3, 3);
                sb.Append(" ↓");
                sb.AppendLine();
            }

            return new CommonMessageResponse(sb.ToString().Trim('\n').Trim('\r'), messageObj);
        }
    }
}
