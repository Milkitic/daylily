using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.Plugin.Message
{
    public class Ping : Application, IApplication
    {
        public Ping()
        {
            appType = AppType.Public;
        }
        public string Execute(string @params, string user, string group, bool isRoot)
        {
            if (group != null) // 不给予群聊权限
                return null;
            return "pong";
        }
    }
}
