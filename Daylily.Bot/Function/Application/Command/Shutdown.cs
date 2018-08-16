using System;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Common;

namespace Daylily.Bot.Function.Application.Command
{
    [Name("强制停止")]
    [Author("yf_extension")]
    [Version(0, 1, 0, PluginVersion.Stable)]
    [Help("此命令会立刻结束程序进程。", HelpType = PermissionLevel.Root)]
    [Command("sdown")]
    public class Shutdown : CommandPlugin
    {
        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            if (messageObj.PermissionLevel != PermissionLevel.Root)
                return new CommonMessageResponse(LoliReply.RootOnly, messageObj, true);
            Environment.Exit(0);
            return null;
        }
    }
}
