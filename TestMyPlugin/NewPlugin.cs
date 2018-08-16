using System;
using System.Collections.Generic;
using System.Text;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Common;
using Daylily.Common.Utils.LoggerUtils;

namespace TestMyPlugin
{
    class NewPlugin: CommandPlugin
    {
        public override void Initialize(string[] args)
        {
            Console.WriteLine(string.Join('\n', args));
        }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            Logger.Info(messageObj.UserId);
            Logger.Info(messageObj.GroupId);
            Logger.Info(messageObj.Message);

            return new CommonMessageResponse("Test", messageObj);
        }
    }
}
