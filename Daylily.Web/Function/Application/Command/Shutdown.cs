using Daylily.Common.Models;
using System;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Web.Function.Application.Command
{
    public class Shutdown : AppConstruct
    {
        public override string Name => "强行停止黄花菜";
        public override string Author => "yf_extension";
        public override PluginVersion Version => PluginVersion.Stable;
        public override string VersionNumber => "1.0";
        public override string Description => "收到消息就自毁";
        public override string Command => "sdown";
        public override AppType AppType => AppType.Command;

        public override void OnLoad(string[] args)
        {
            //throw new NotImplementedException();
        }

        public override CommonMessageResponse OnExecute(CommonMessage messageObj)
        {
            if (messageObj.PermissionLevel != PermissionLevel.Root)
                return new CommonMessageResponse(LoliReply.RootOnly, messageObj, true);
            Environment.Exit(0);
            return null;
        }
    }
}
