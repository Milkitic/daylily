using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.CoolQ.Message;

namespace Daylily.Plugin.Core
{
    [Name("猜拳/掷骰子")]
    [Author("yf_extension")]
    [Version(0, 1, 0, PluginVersion.Stable)]
    [Help("发送掷骰子或猜拳魔法表情。")]
    [Command("dice", "rps")]
    public class RpsDice : CommandPlugin
    {
        public override CommonMessageResponse OnMessageReceived(CoolQNavigableMessage navigableMessageObj)
        {
            switch (navigableMessageObj.Command)
            {
                case "dice":
                    return new CommonMessageResponse(new Dice().ToString(), navigableMessageObj);
                case "rps":
                    return new CommonMessageResponse(new Rps().ToString(), navigableMessageObj);
                default:
                    return null;
            }
        }
    }
}
