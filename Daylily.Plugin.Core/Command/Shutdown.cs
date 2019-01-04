using Daylily.Bot.Enum;
using System;
using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.CoolQ.Message;

namespace Daylily.Plugin.Core
{
    [Name("强制停止")]
    [Author("yf_extension")]
    [Version(0, 1, 0, PluginVersion.Stable)]
    [Help("此命令会立刻结束程序进程。", Authority = Authority.Root)]
    [Command("sdown")]
    public class Shutdown : CommandPlugin
    {
        public override void OnInitialized(string[] args)
        {

        }

        public override CommonMessageResponse OnMessageReceived(CoolQNavigableMessage navigableMessageObj)
        {
            if (navigableMessageObj.Authority != Authority.Root)
                return new CommonMessageResponse(LoliReply.RootOnly, navigableMessageObj, true);
            Environment.Exit(0);
            return null;
        }
    }
}
