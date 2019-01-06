using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.CoolQ;
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

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            var plugins = DaylilyCore.Current.PluginManager.ApplicationInstances
                .OrderByDescending(k => k.MiddlewareConfig?.Priority);
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Application plugins:");

            string prefix = "";
            int prevPrior = int.MinValue;
            foreach (var plugin in plugins)
            {
                string prior = "";
                if (prevPrior != plugin.MiddlewareConfig.Priority)
                {
                    prevPrior = plugin.MiddlewareConfig.Priority;
                    prior = $" ({prevPrior})";
                }
                sb.Append(plugin.Name + prior + " ➡️ ");
                if (plugin.RunInMultiThreading)
                {

                }
                else
                {
                    sb.Remove(sb.Length - 4, 4);
                    sb.Append(" ↩️");
                    sb.AppendLine();
                    prefix += "    ";
                    sb.Append(prefix);
                }
            }

            sb.Remove(sb.Length - 4, 4);
            sb.Append(" ↩️");
            sb.AppendLine();


            sb.AppendLine("Command plugins");

            return routeMsg.ToSource(sb.ToString().Trim('\n').Trim('\r'));
        }
    }
}
