using System;
using Daylily.Common.Models;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Plugin.Core.Command
{
    public class Help : AppConstruct
    {
        public override string Name => "黄花菜帮助";
        public override string Author => "yf_extension";
        public override PluginVersion Version => PluginVersion.Stable;
        public override string VersionNumber => "1.0";
        public override string Description => null;
        public override string Command => "help";
        public override AppType AppType => AppType.Command;

        public override void OnLoad(string[] args)
        {
        }

        public override CommonMessageResponse OnExecute(in CommonMessage messageObj)
        {
            return new CommonMessageResponse("太多了哇..都在这里：https://www.zybuluo.com/milkitic/note/1130078", messageObj);
        }
    }
}
