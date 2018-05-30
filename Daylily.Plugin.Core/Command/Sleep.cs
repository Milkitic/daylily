using System;
using Daylily.Common.Interface.CQHttp;
using Daylily.Common.Models;

namespace Daylily.Plugin.Core.Command
{
    public class Sleep : AppConstruct
    {
        public override CommonMessageResponse Execute(CommonMessage message)
        {
            if (message.GroupId == "133605766")
                return null;
            if (message.GroupId == null)
                return null;
            if (message.Parameter.Trim() == "")
                return new CommonMessageResponse("要睡多少小时呀? 你不写是要30循吗??", message, true);

            DateTime dt = new DateTime();
            if (!double.TryParse(message.Parameter, out double result))
                return new CommonMessageResponse("我只要一个数表示小时，支持小数", message, true);
            if (result > 12) result = 12;
            else if (result < 0.5) result = 0.5;
            dt = dt.AddHours(result);
            int s = (int)(dt.Ticks / 10000000);
            CqApi.SetGroupBan(message.GroupId, message.UserId, s);
            string msg = "祝你一觉睡到" + DateTime.Now.AddHours(result).ToString("HH:mm") + " :D";

            return new CommonMessageResponse(msg, message, true);
        }
    }
}
