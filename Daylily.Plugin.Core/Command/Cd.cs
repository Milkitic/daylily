using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Plugin.Core.Command
{
    [Name("文件浏览")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Alpha)]
    [Help("浏览磁盘目录")]
    [Command("cd")]
    class Cd : CommandApp
    {
        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            if (messageObj.GroupId != null) // 不给予群聊权限
                return null;

            if (messageObj.PermissionLevel != PermissionLevel.Root)
                return new CommonMessageResponse(LoliReply.RootOnly, messageObj);
            StringBuilder sb = new StringBuilder();

            DirectoryInfo di = new DirectoryInfo(messageObj.Parameter);
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

            return new CommonMessageResponse(sb.ToString().Trim(), messageObj);
        }
    }
}
