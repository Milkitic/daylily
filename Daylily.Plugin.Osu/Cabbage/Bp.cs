using Daylily.Bot.Backend;
using Daylily.CoolQ;
using Daylily.CoolQ.Messaging;
using System;
using System.Threading.Tasks;
using Daylily.CoolQ.Plugin;

namespace Daylily.Plugin.Osu.Cabbage
{
    [Name("Bp查询（白菜）")]
    [Author("yf_extension")]
    [Version(2, 0, 1, PluginVersion.Alpha)]
    [Help("详情问白菜。")]
    [Command("bpme", "bp")]
    public class Bp : CoolQCommandPlugin
    {
        public override Guid Guid => new Guid("e6f44b37-be2b-418e-b0a3-f1c461294b04");

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            CabbageCommon.MessageQueue.Enqueue(routeMsg);

            bool isTaskFree = CabbageCommon.TaskQuery == null || CabbageCommon.TaskQuery.IsCanceled ||
                              CabbageCommon.TaskQuery.IsCompleted;
            if (isTaskFree)
                CabbageCommon.TaskQuery = Task.Run(() => CabbageCommon.Query());

            return null;
        }
    }
}
