using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Linq;
using System.Text;

namespace Daylily.Plugin.Kernel
{
    [Name("插件检查")]
    [Author("yf_extension")]
    [Version(2, 0, 1, PluginVersion.Stable)]
    [Help("检查插件的情况。", Authority = Authority.Root)]
    [Command("check")]
    public class PluginChecker : CoolQCommandPlugin
    {
        public override Guid Guid => new Guid("14f02b6a-44d3-4064-9e9d-c04796793ec7");

        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMessageObj)
        {
            var grouped = DaylilyCore.Current.PluginManager.ApplicationInstances
                .OrderByDescending(k => k.MiddlewareConfig?.Priority)
                .GroupBy(k => k.MiddlewareConfig?.Priority);
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

            return routeMessageObj.ToSource(sb.ToString().Trim('\n').Trim('\r'));
        }
    }
}
