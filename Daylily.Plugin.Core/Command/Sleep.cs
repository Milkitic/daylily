using System;
using Daylily.Common.Interface.CQHttp;
using Daylily.Common.Models;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Plugin.Core.Command
{
    public class Sleep : AppConstruct
    {
        public override string Name => "自我禁言";
        public override string Author => "yf_extension";
        public override PluginVersion Version => PluginVersion.Stable;
        public override string VersionNumber => "1.0";
        public override string Description => "只允许30分钟到12小时";
        public override string Command => "sleep,slip";
        public override AppType AppType => AppType.Command;

        public override void OnLoad(string[] args)
        {
            throw new NotImplementedException();
        }

        public override CommonMessageResponse OnExecute(CommonMessage messageObj)
        {
            if (messageObj.GroupId == "133605766")
                return null;
            if (messageObj.GroupId == null)
                return null;
            if (messageObj.Parameter.Trim() == "")
                return new CommonMessageResponse("要睡多少小时呀? 你不写是要30循吗??", messageObj, true);

            DateTime dt = new DateTime();
            if (!double.TryParse(messageObj.Parameter, out double result))
                return new CommonMessageResponse("我只要一个数表示小时，支持小数", messageObj, true);
            if (result > 12) result = 12;
            else if (result < 0.5) result = 0.5;
            dt = dt.AddHours(result);
            int s = (int)(dt.Ticks / 10000000);
            CqApi.SetGroupBan(messageObj.GroupId, messageObj.UserId, s);
            string msg = "祝你一觉睡到" + DateTime.Now.AddHours(result).ToString("HH:mm") + " :D";

            return new CommonMessageResponse(msg, messageObj, true);
        }
    }
}
