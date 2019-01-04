using Daylily.Bot.Enum;
using System;
using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;

namespace Daylily.Plugin.Core
{
    [Name("强制停止")]
    [Author("yf_extension")]
    [Version(2, 0, 0, PluginVersion.Stable)]
    [Help("此命令会立刻结束程序进程。", Authority = Authority.Root)]
    [Command("sdown")]
    public class Shutdown : CoolQCommandPlugin
    {
        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMsg)
        {
            if (routeMsg.Authority != Authority.Root)
                return routeMsg.ToSource(LoliReply.RootOnly, true);
            Environment.Exit(0);
            return null;
        }
    }
}
