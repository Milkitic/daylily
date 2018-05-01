using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Daylily.Plugin.Command
{
    class Cd : Application
    {
        public override string Execute(string @params, string user, string group, PermissionLevel currentLevel, ref bool ifAt)
        {
            ifAt = false;
            if (group != null) // 不给予群聊权限
                return null;
            CurrentLevel = currentLevel;
            if (currentLevel != PermissionLevel.Root)
                return "只有超级管理员才能发动此技能…";
            StringBuilder sb = new StringBuilder();

            DirectoryInfo di = new DirectoryInfo(@params);
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

            return sb.ToString().Trim();
        }
    }
}
