using System;
using System.Collections.Generic;
using System.Text;
using Daylily.Bot.Models;

namespace Daylily.Bot.Backend.Plugins
{
    public abstract class ServicePlugin : Plugin
    {
        public sealed override PluginType PluginType => PluginType.Service;
        public override MiddlewareConfig MiddlewareConfig { get; } = new BackendConfig();
    }
}
