using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.PluginBase
{
    public abstract class ResponsivePlugin : CqPlugin
    {
        public override PluginType PluginType => PluginType.Unknown;
        public abstract bool RunInMultiThreading { get; }
        public abstract bool RunInMultipleInstances { get; }
        public abstract CommonMessageResponse OnMessageReceived(CommonMessage messageObj);
    }
}
