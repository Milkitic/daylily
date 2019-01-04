using Daylily.Bot.Message;
using Daylily.CoolQ.Interface.CqHttp;
using System;
using Daylily.Bot.Backend;
using Daylily.CoolQ.Message;

namespace Daylily.Plugin.ShaDiao
{
    [Name("自助禁言")]
    [Author("yf_extension")]
    [Version(0, 1, 0, PluginVersion.Stable)]
    [Help("当Daylily是管理员时，将命令发送者禁言（30分钟到12小时）。")]
    [Command("sleep")]
    public class Sleep : CommandPlugin
    {
        [FreeArg(Default = -1)]
        [Help("要禁言的时长，小时为单位，支持小数")]
        public double SleepTime { get; set; }

        public override void OnInitialized(string[] args)
        {

        }

        public override CommonMessageResponse OnMessageReceived(CoolQNavigableMessage navigableMessageObj)
        {
            if (navigableMessageObj.GroupId == "133605766")
                return null;
            if (navigableMessageObj.GroupId == null)
                return null;
            if (navigableMessageObj.ArgString.Trim() == "")
                return new CommonMessageResponse("要睡多少小时呀??", navigableMessageObj, true);

            double sleepTime;
            if (SleepTime > 12) sleepTime = 12;
            else if (SleepTime < 0.5) sleepTime = 0.5;
            else if (SleepTime > 0) sleepTime = SleepTime;
            else return new CommonMessageResponse("穿越是不可以的……", navigableMessageObj, true);

            DateTime dt = new DateTime();
            dt = dt.AddHours(sleepTime);
            int s = (int)(dt.Ticks / 10000000);
            CqApi.SetGroupBan(navigableMessageObj.GroupId, navigableMessageObj.UserId, s);
            string msg = "祝你一觉睡到" + DateTime.Now.AddHours(sleepTime).ToString("HH:mm") + " :D";

            return new CommonMessageResponse(msg, navigableMessageObj, true);
        }
    }
}
