using System.Threading.Tasks;
using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.CoolQ.Message;

namespace Daylily.Plugin.Osu.Cabbage
{
    [Name("Stat查询（白菜）")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Alpha)]
    [Help("详情问白菜。")]
    [Command("statme", "stat")]
    public class Stat : CommandPlugin
    {
        public override void OnInitialized(string[] args)
        {
        }

        public override CommonMessageResponse OnMessageReceived(CoolQNavigableMessage navigableMessageObj)
        {
            CabbageCommon.MessageQueue.Enqueue(navigableMessageObj);

            bool isTaskFree = CabbageCommon.TaskQuery == null || CabbageCommon.TaskQuery.IsCanceled ||
                              CabbageCommon.TaskQuery.IsCompleted;
            if (isTaskFree)
                CabbageCommon.TaskQuery = Task.Run(() => CabbageCommon.Query());

            return null;
        }
    }
}
