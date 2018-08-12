using System;
using System.Collections.Generic;
using System.Text;
using Daylily.Common.Models;
using Daylily.Common.Models.Interface;
using Daylily.Common.Utils.LogUtils;

namespace TestMyPlugin
{
    class NewPlugin: CommandApp
    {
        public override void Initialize(string[] args)
        {
            Console.WriteLine(string.Join('\n', args));
        }

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            Logger.Info(messageObj.UserId);
            Logger.Info(messageObj.GroupId);
            Logger.Info(messageObj.Message);

            return new CommonMessageResponse("Test", messageObj);
        }
    }
}
