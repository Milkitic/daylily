using DaylilyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Function.Application.Command
{
    public class Help : Application
    {
        public override string Execute(string message, string user, string group, PermissionLevel currentLevel, ref bool ifAt)
        {
            return "太多了哇..都在这里：https://www.zybuluo.com/milkitic/note/1130078";
        }
    }
}
