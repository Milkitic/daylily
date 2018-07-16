using System;
using System.Threading.Tasks;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Common.Function.Application.Command
{
    [Name("强行停止黄花菜")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Stable)]
    [Help("收到消息立刻结束进程")]
    [Command("sdown")]
    public class Shutdown : CommandApp
    {
        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            if (messageObj.PermissionLevel != PermissionLevel.Root)
                return new CommonMessageResponse(LoliReply.RootOnly, messageObj, true);
            Environment.Exit(0);
            return null;
        }
    }
}
