using System;
using System.Threading.Tasks;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Plugin.Core.Command
{
    [Name("Ping")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Stable)]
    [Help("返回pong")]
    [Command("ping")]
    public class Ping : CommandApp
    {
        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            if (messageObj.GroupId != null) // 不给予群聊权限
                return null;
            return new CommonMessageResponse("pong", messageObj);
        }
    }
}
