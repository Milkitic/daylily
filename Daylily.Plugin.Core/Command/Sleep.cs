using System;
using System.Threading.Tasks;
using Daylily.Common.Interface.CQHttp;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Plugin.Core.Command
{
    [Name("自助禁言")]
    [Author("yf_extension")]
    [Version(0, 1, 0, PluginVersion.Stable)]
    [Help("当Daylily是管理员时，将命令发送者禁言（30分钟到12小时）。")]
    [Command("sleep", "slip")]
    public class Sleep : CommandApp
    {
        [FreeArg(Default = -1)]
        [Help("要禁言的时长，小时为单位，支持小数")]
        public double SleepTime { get; set; }

        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            if (messageObj.GroupId == "133605766")
                return null;
            if (messageObj.GroupId == null)
                return null;
            if (messageObj.ArgString.Trim() == "")
                return new CommonMessageResponse("要睡多少小时呀??", messageObj, true);

            double sleepTime;
            if (SleepTime > 12) sleepTime = 12;
            else if (SleepTime < 0.5) sleepTime = 0.5;
            else if (SleepTime > 0) sleepTime = SleepTime;
            else return new CommonMessageResponse("穿越是不可以的……", messageObj, true);

            DateTime dt = new DateTime();
            dt = dt.AddHours(sleepTime);
            int s = (int)(dt.Ticks / 10000000);
            CqApi.SetGroupBan(messageObj.GroupId, messageObj.UserId, s);
            string msg = "祝你一觉睡到" + DateTime.Now.AddHours(sleepTime).ToString("HH:mm") + " :D";

            return new CommonMessageResponse(msg, messageObj, true);
        }
    }
}
