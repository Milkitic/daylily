using Daylily.Bot.Backend;
using Daylily.CoolQ;
using Daylily.CoolQ.CoolQHttp;
using Daylily.CoolQ.Messaging;
using System;
using Daylily.CoolQ.Plugin;

namespace Daylily.Plugin.Fun
{
    [Name("自助禁言")]
    [Author("yf_extension")]
    [Version(2, 0, 0, PluginVersion.Stable)]
    [Help("当Daylily是管理员时，将命令发送者禁言（30分钟到12小时）。")]
    [Command("sleep")]
    public class Sleep : CoolQCommandPlugin
    {
        public override Guid Guid => new Guid("4c729d16-3954-4e70-ad4c-8a0ea72efe1a");

        [FreeArg(Default = -1)]
        [Help("要禁言的时长，小时为单位，支持小数")]
        public double SleepTime { get; set; }

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            if (routeMsg.GroupId == "133605766")
                return null;
            if (routeMsg.GroupId == null)
                return null;
            if (routeMsg.ArgString.Trim() == "")
                return routeMsg.ToSource("要睡多少小时呀??", true);

            double sleepTime;
            if (SleepTime > 12) sleepTime = 12;
            else if (SleepTime < 0.5) sleepTime = 0.5;
            else if (SleepTime > 0) sleepTime = SleepTime;
            else return routeMsg.ToSource("穿越是不可以的……", true);

            DateTime dt = new DateTime();
            dt = dt.AddHours(sleepTime);
            int s = (int)(dt.Ticks / 10000000);
            CoolQHttpApiClient.SetGroupBan(routeMsg.GroupId, routeMsg.UserId, s);
            string msg = "祝你一觉睡到" + DateTime.Now.AddHours(sleepTime).ToString("HH:mm") + " :D";

            return routeMsg.ToSource(msg, true);
        }
    }
}
