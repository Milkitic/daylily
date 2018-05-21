using System.IO;
using System.Text;
using Daylily.Common.Models;

namespace Daylily.Plugin.Core.Command
{
    class Cd : AppConstruct
    {
        public override CommonMessageResponse Execute(CommonMessage message)
        {
            if (message.GroupId != null) // 不给予群聊权限
                return null;

            if (message.PermissionLevel != PermissionLevel.Root)
                return new CommonMessageResponse("只有超级管理员才能发动此技能…", message);
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
