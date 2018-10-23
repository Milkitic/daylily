using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Daylily.Bot;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ;
using Daylily.CoolQ.Models;
using Daylily.Osu.Database.BLL;
using Daylily.Osu.Database.Model;

namespace Daylily.Plugin.Osu.Cabbage
{
    [Name("Cost查询（白菜）")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Alpha)]
    [Help("详情问白菜。")]
    [Command("costme", "cost")]
    public class Cost : CommandPlugin
    {
        public override void Initialize(string[] args)
        {
        }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            CabbageCommon.MessageQueue.Enqueue(messageObj);

            bool isTaskFree = CabbageCommon.TaskQuery == null || CabbageCommon.TaskQuery.IsCanceled ||
                              CabbageCommon.TaskQuery.IsCompleted;
            if (isTaskFree)
                CabbageCommon.TaskQuery = Task.Run(() => CabbageCommon.Query());

            return null;
        }
    }
}
