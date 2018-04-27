using DaylilyWeb.Interface.CQHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Functions.Applications
{
    public class Test : Application
    {
        HttpApi CqApi = new HttpApi();

        public override string Execute(string message, string user, string group, bool isRoot, ref bool ifAt)
        {
            var abc = CqApi.GetGroupList();
            return null;
        }
    }
}
