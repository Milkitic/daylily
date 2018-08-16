using Daylily.Bot.Enum;
using Daylily.Bot.Models;

namespace Daylily.Bot.PluginBase
{
    public abstract class ApplicationPlugin : Plugin
    {
        public sealed override PluginType PluginType => PluginType.Application;

        public abstract CommonMessageResponse Message_Received(CommonMessage messageObj);
    }
}
