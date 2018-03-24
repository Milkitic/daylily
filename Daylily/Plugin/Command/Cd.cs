using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Daylily.Plugin.Command
{
    class Cd : Application, IApplication
    {
        public Cd()
        {
            appType = AppType.RequirePermission;
        }
        public string Execute(string @params, string user, string group, bool isRoot)
        {
            if (group != null) // 不给予群聊权限
                return null;
            this.isRoot = isRoot;
            if (appType == AppType.RequirePermission && !isRoot)
                throw new InvalidOperationException("您不能干这事儿");
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
