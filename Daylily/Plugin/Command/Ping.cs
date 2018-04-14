using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.Plugin.Command
{
    public class Ping : Application
    {
        public Ping()
        {
            appType = AppType.Public;
        }
        public override string Execute(string @params, string user, string group, bool isRoot, ref bool ifAt)
        {
            if (group != null) // 不给予群聊权限
                return null;
            return "pong";
        }
    }
}
