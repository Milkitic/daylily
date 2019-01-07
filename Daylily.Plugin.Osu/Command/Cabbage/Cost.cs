using Daylily.Bot.Backend;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Threading.Tasks;

namespace Daylily.Plugin.Osu.Cabbage
{
    [Name("Cost查询（白菜）")]
    [Author("yf_extension")]
    [Version(2, 0, 1, PluginVersion.Alpha)]
    [Help("详情问白菜。")]
    [Command("costme", "cost")]
    public class Cost : CoolQCommandPlugin
    {
        public override Guid Guid => new Guid("acb26399-f29d-40f2-aeea-9f95259516ff");

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
