using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Backend.Plugins
{
    public abstract class ServicePlugin : Plugin
    {
        public sealed override PluginType PluginType => PluginType.Service;
        public override MiddlewareConfig MiddlewareConfig { get; } = new BackendConfig();
    }
}
