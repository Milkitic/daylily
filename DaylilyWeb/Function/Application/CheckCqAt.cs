using DaylilyWeb.Assist;
using DaylilyWeb.Interface.CQHttp;
using DaylilyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Function.Application
{
    public class CheckCqAt : Application
    {
        public override string Execute(string message, string user, string group, PermissionLevel currentLevel, ref bool ifAt, long messageId)
        {
            if (group == "133605766")
                if (DateTime.Now.Hour < 22 && DateTime.Now.Hour > 6)
                    return null;
            string[] ids = CQCode.GetAt(message);
            if (ids != null && ids.Contains("2181697779"))
            {
                ifAt = true;
                return "";
            }
            return null;
        }
    }
}
