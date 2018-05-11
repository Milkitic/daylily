using DaylilyWeb.Interface.CQHttp;
using DaylilyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Function.Application
{
    public class Test : Application
    {
        HttpApi CqApi = new HttpApi();

        public override string Execute(string message, string user, string group, PermissionLevel currentLevel, ref bool ifAt, long messageId)
        {
            var abc = CqApi.GetGroupList();
            return null;
        }
    }
}
