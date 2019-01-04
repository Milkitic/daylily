using Daylily.Bot.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Backend.Plugins
{
    public abstract class ResponsivePlugin : Plugin
    {
        public override PluginType PluginType => PluginType.Unknown;
        public abstract bool RunInMultiThreading { get; }
        public abstract bool RunInMultipleInstances { get; }

        public abstract NavigableMessage OnMessageReceived(NavigableMessage navigableMessage);
    }
}
