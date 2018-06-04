using System;
using System.IO;
using System.Text;
using Daylily.Common.Models;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Plugin.Core.Command
{
    class Cd : AppConstruct
    {
        public override string Name => "文件浏览器";
        public override string Author => "yf_extension";
        public override PluginVersion Version => PluginVersion.Alpha;
        public override string VersionNumber => "1.0";
        public override string Description => "浏览磁盘目录";
        public override string Command => "cd";
        public override AppType AppType => AppType.Command;

        public override void OnLoad(string[] args)
        {
            throw new NotImplementedException();
        }

        public override CommonMessageResponse OnExecute(CommonMessage message)
        {
            if (message.GroupId != null) // 不给予群聊权限
                return null;

            if (message.PermissionLevel != PermissionLevel.Root)
                return new CommonMessageResponse(LoliReply.RootOnly, message);
            StringBuilder sb = new StringBuilder();

            DirectoryInfo di = new DirectoryInfo(message.Parameter);
            var files = di.GetFiles();
            var dirs = di.GetDirectories();
            foreach (var item in dirs)
            {
                sb.Append(item.Name + "*   ");
            }

            foreach (var item in files)
            {
                sb.Append(item.Name + "   ");
            }

            return new CommonMessageResponse(sb.ToString().Trim(), message);
        }
    }
}
