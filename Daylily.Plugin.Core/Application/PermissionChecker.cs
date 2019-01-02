using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Common.Utils.LoggerUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Daylily.Plugin.Core.Application
{
    public class PermissionChecker : ApplicationPlugin
    {
        public override bool RunInMultiThreading => false;
        public override CommonMessageResponse OnMessageReceived(CommonMessage messageObj)
        {
            //todo: not implemented
            return null;
        }
    }
}
