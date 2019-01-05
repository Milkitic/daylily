using Daylily.Bot.Backend;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.CoolQ
{
    public static class PluginTypeExtension
    {
        public static string ToBotString(this PluginType pluginType)
        {
            switch (pluginType)
            {
                case PluginType.Command:
                    return "命令插件";
                case PluginType.Application:
                    return "应用插件";
                case PluginType.Service:
                    return "服务插件";
                case PluginType.Event:
                    return "事件插件";
                case PluginType.Unknown:
                    return "未知插件";
                default:
                    throw new ArgumentOutOfRangeException(nameof(pluginType), pluginType, null);
            }
        }
    }
}
