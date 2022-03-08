using System.ComponentModel;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Platforms.GoCqHttp.Messaging.CqCodes;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins;

[PluginIdentifier("d6ba1003-4c02-46d6-94c5-52b737f7b967", "猜拳/掷骰子")]
[Author("yf_extension")]
[Version("2.0.0")]
[Description("发送掷骰子或猜拳魔法表情。")]
internal class RpsDice : BasicPlugin
{
    [CommandHandler("rps")]
    [Description("掷骰子")]
    public IResponse Rps()
    {
        return Reply(CQRps.Instance);
    }

    [CommandHandler("dice")]
    [Description("猜拳")]
    public IResponse Dice()
    {
        return Reply(CQDice.Instance);
    }
}