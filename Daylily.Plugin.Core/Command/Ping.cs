using System;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;

namespace Daylily.Plugin.Core.Command
{
    [Name("Ping")]
    [Author("yf_extension")]
    [Version(0, 1, 0, PluginVersion.Stable)]
    [Help("返回\"pong\"。")]
    [Command("ping")]
    public class Ping : CommandPlugin
    {
        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            if (messageObj.GroupId != null) // 不给予群聊权限
                return null;
            return new CommonMessageResponse("pong", messageObj);
        }
    }
}
