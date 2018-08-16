using System.Threading;
using System.Threading.Tasks;
using Daylily.Bot;
using Daylily.Bot.PluginBase;

namespace Daylily.Plugin.Core.Service
{
    class UpdateGroupList : ServicePlugin
    {
        public override void RunTask(string[] args)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    SaveSettings(MessageHandler.GroupInfo);
                    Thread.Sleep(1000 * 60);
                }
            });
        }
    }
}
