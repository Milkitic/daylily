using System;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ.Models.CqResponse;

namespace Daylily.Plugin.ShaDiao
{
    class Program
    {
        static void Main(string[] args)
        {
            // 引用添加项目Daylily.Common
            Panda newPlugin = new Panda();
            newPlugin.OnInitialized(args);
            CommonMessage cm = new CommonMessage()
            {
                GroupId = "123456788",
                UserId = "2241521134",
                RawMessage = "SB",
                MessageType = MessageType.Group,
                Group = new GroupMsg(),
            };

            Logger.Success("收到：" + newPlugin.OnMessageReceived(cm).Message);
            Console.ReadKey();
        }
    }
}
