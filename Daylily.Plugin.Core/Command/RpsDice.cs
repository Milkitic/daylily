using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Plugin.Core.Command
{
    [Name("猜拳/掷骰子")]
    [Author("yf_extension")]
    [Version(0, 1, 0, PluginVersion.Stable)]
    [Help("发送掷骰子或猜拳魔法表情。")]
    [Command("dice", "rps")]
    public class RpsDice : CommandPlugin
    {
        public override void OnInitialized(string[] args)
        {
        }

        public override CommonMessageResponse OnMessageReceived(CommonMessage messageObj)
        {
            switch (messageObj.Command)
            {
                case "dice":
                    return new CommonMessageResponse(new CoolQ.Dice().ToString(), messageObj);
                case "rps":
                    return new CommonMessageResponse(new CoolQ.Rps().ToString(), messageObj);
                default:
                    return null;
            }
        }
    }
}
