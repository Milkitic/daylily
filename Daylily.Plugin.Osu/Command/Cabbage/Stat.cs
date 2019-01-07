using Daylily.Bot.Backend;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Threading.Tasks;

namespace Daylily.Plugin.Osu.Cabbage
{
    [Name("Stat查询（白菜）")]
    [Author("yf_extension")]
    [Version(2, 0, 1, PluginVersion.Alpha)]
    [Help("详情问白菜。")]
    [Command("statme", "stat")]
    public class Stat : CoolQCommandPlugin
    {
        public override Guid Guid => new Guid("21d73cbc-782d-4ba7-8bf9-86ee7d5cc6bc");

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
