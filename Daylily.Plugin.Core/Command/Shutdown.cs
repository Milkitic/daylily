using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using System;

namespace Daylily.Plugin.Core
{
    [Name("强制停止")]
    [Author("yf_extension")]
    [Version(0, 1, 0, PluginVersion.Stable)]
    [Help("此命令会立刻结束程序进程。", Authority = Bot.Enum.Authority.Root)]
    [Command("sdown")]
    public class Shutdown : CommandPlugin
    {
        public override void OnInitialized(string[] args)
        {

        }

        public override CommonMessageResponse OnMessageReceived(CommonMessage messageObj)
        {
            if (messageObj.Authority != Bot.Enum.Authority.Root)
                return new CommonMessageResponse(LoliReply.RootOnly, messageObj, true);
            Environment.Exit(0);
            return null;
        }
    }
}
