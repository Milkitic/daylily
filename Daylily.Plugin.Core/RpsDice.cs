using Daylily.Bot.Backend;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;

namespace Daylily.Plugin.Basic
{
    [Name("猜拳/掷骰子")]
    [Author("yf_extension")]
    [Version(2, 0, 0, PluginVersion.Stable)]
    [Help("发送掷骰子或猜拳魔法表情。")]
    [Command("dice", "rps")]
    public class RpsDice : CoolQCommandPlugin
    {
        public override Guid Guid => new Guid("d6ba1003-4c02-46d6-94c5-52b737f7b967");

        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMsg)
        {
            switch (routeMsg.Command)
            {
                case "dice":
                    return routeMsg.ToSource(new Dice());
                case "rps":
                    return routeMsg.ToSource(new Rps());
                default:
                    return null;
            }
        }
    }
}
