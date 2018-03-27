using DaylilyWeb.Database.BLL;
using DaylilyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Functions.Applications
{
    public class MyInfo : Application, IApplication
    {
        public MyInfo()
        {
            appType = AppType.Public;
        }
        public string Execute(string @params, string user, string group, bool isRoot)
        {
            //if (group != null) // 不给予群聊权限
            //    return null;
            BllUserRole bllUserRole = new BllUserRole();
            var ok = bllUserRole.GetUserRoleByQQ(long.Parse(user));
            if(ok.Count !=0)
            return ok[0].CurrentUname;
            return "你都还没有绑你查个jb啊，用白菜的!setid +id";
        }
    }
}
