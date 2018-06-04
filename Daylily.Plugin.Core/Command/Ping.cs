using System;
using Daylily.Common.Models;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Plugin.Core.Command
{
    public class Ping : AppConstruct
    {
        public override string Name => "Ping";
        public override string Author => "yf_extension";
        public override PluginVersion Version => PluginVersion.Stable;
        public override string VersionNumber => "1.0";
        public override string Description => "返回pong";
        public override string Command => "ping";
        public override AppType AppType => AppType.Command;

        public override void OnLoad(string[] args)
        {
            throw new NotImplementedException();
        }

        public override CommonMessageResponse OnExecute(CommonMessage message)
        {
            if (message.GroupId != null) // 不给予群聊权限
                return null;
            return new CommonMessageResponse("pong", message);
        }
    }
}
