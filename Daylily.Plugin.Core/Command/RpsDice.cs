using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;

namespace Daylily.Plugin.Core
{
    [Name("猜拳/掷骰子")]
    [Author("yf_extension")]
    [Version(2, 0, 0, PluginVersion.Stable)]
    [Help("发送掷骰子或猜拳魔法表情。")]
    [Command("dice", "rps")]
    public class RpsDice : CoolQCommandPlugin
    {
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
