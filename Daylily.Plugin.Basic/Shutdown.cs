using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;

namespace Daylily.Plugin.Basic
{
    [Name("强制停止")]
    [Author("yf_extension")]
    [Version(2, 0, 0, PluginVersion.Stable)]
    [Help("此命令会立刻结束程序进程。", Authority = Authority.Root)]
    [Command("sdown")]
    public class Shutdown : CoolQCommandPlugin
    {
        public override Guid Guid => new Guid("ee3fc222-b05d-403b-b8c2-542a61b72a4d");

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
