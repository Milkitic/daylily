using Daylily.Bot.Backend.Plugins;
using Daylily.Bot.Message;
using Daylily.CoolQ.Message;

namespace Daylily.CoolQ.Plugins
{
    public abstract class CoolQApplicationPlugin : ApplicationPlugin
    {
        public abstract CoolQNavigableMessage OnMessageReceived(CoolQNavigableMessage navigableMessageObj);

        public override NavigableMessage OnMessageReceived(NavigableMessage navigableMessage)
        {
            return OnMessageReceived((CoolQNavigableMessage)navigableMessage);
        }
    }
}
