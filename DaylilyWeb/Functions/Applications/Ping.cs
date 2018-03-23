using DaylilyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Functions.Applications
{
    public class Ping : Application, IApplication
    {
        public Ping()
        {
            appType = AppType.Public;
        }
        public string Execute(string @params, string user, string group)
        {
            if (group != null) // 不给予群聊权限
                return null;
            return "pong";
        }
    }
}
