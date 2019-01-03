using Daylily.Bot.Enum;
using Daylily.Bot.Models;

namespace Daylily.Bot.PluginBase
{
    public abstract class ServicePlugin : Plugin
    {
        public sealed override PluginType PluginType => PluginType.Service;
        public override BackendConfig BackendConfig { get; } = new BackendConfig();
        public abstract void Execute(string[] args);
    }
}
