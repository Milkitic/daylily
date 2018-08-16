using System;
using System.IO;
using System.Text;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;

namespace Daylily.Plugin.Core.Command
{
    [Name("文件浏览")]
    [Author("yf_extension")]
    [Version(0, 1, 0, PluginVersion.Alpha)]
    [Help("浏览磁盘目录。", HelpType = PermissionLevel.Root)]
    [Command("cd")]
    public class Cd : CommandPlugin
    {
        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            if (messageObj.GroupId != null) // 不给予群聊权限
                return null;

            if (messageObj.PermissionLevel != PermissionLevel.Root)
                return new CommonMessageResponse(LoliReply.RootOnly, messageObj);
            StringBuilder sb = new StringBuilder();

            DirectoryInfo di = new DirectoryInfo(messageObj.ArgString);
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
