using Daylily.Bot.Enum;

namespace Daylily.Bot.PluginBase
{
    public abstract class ServicePlugin : Plugin
    {
        public sealed override PluginType PluginType => PluginType.Service;
        public abstract void RunTask(string[] args);
    }
}
