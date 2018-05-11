using Daylily.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.Plugin.Command
{
    public class Ping : Application
    {
        public override string Execute(string @params, string user, string group, PermissionLevel currentLevel, ref bool ifAt, long messageId)
        {
            if (group != null) // 不给予群聊权限
                return null;
            return "pong";
        }
    }
}
