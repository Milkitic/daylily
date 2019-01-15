using System;
using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;

namespace Daylily.Plugin.Kernel
{
    [Name("强制重启")]
    [Author("yf_extension")]
    [Version(2, 0, 1, PluginVersion.Stable)]
    [Help("此命令会立刻结束程序进程并重启。", Authority = Authority.Root)]
    [Command("reboot")]
    public class Shutdown : CoolQCommandPlugin
    {
        public override Guid Guid => new Guid("ee3fc222-b05d-403b-b8c2-542a61b72a4d");

        public override void AllPlugins_Initialized(StartupConfig startup)
        {
            base.AllPlugins_Initialized(startup);
        }

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            if (routeMsg.CurrentAuthority != Authority.Root)
                return routeMsg.ToSource(DefaultReply.RootOnly, true);
            Environment.Exit(0);
            return null;
        }
    }
}
