using Daylily.Interface.CQHttp;
using Daylily.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.Plugin.Command
{
    public class Sleep : Application
    {
        HttpApi CQApi = new HttpApi();

        public override string Execute(string @params, string user, string group, PermissionLevel currentLevel, ref bool ifAt)
        {
            if (group == null)
                return null;
            ifAt = true;
            if (@params.Trim() == "")
                return "要睡多少小时呀? 你不写是要30循吗??";

            DateTime dt = new DateTime();
            if (!double.TryParse(@params, out double result))
                return "我只要一个数表示小时，支持小数";
            if (result > 12) result = 12;
            else if (result < 0.5) result = 0.5;
            dt = dt.AddHours(result);
            int s = (int)(dt.Ticks / 10000000);
            CQApi.SetGroupBan(group, user, s);
            return "祝你一觉睡到" + DateTime.Now.AddHours(result).ToString("HH:mm") + " :D";
        }
    }
}
