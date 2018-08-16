using System.Threading;
using System.Threading.Tasks;
using Daylily.Bot.PluginBase;

namespace Daylily.Bot.Function.Application.Service
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
