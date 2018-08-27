using System;
using System.Collections.Generic;
using System.Text;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;

namespace Daylily.Plugin.ShaDiao.Application
{
    [Name("数羊")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Stable)]
    [Help("数羊测试。")]
    public class Count : ApplicationPlugin
    {
        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            throw new NotImplementedException();
        }
    }
}
