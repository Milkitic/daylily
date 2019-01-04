using Daylily.Bot.Message;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ.Models.CqResponse;
using System;
using Daylily.CoolQ.Message;

namespace Daylily.Plugin.ShaDiao
{
    class Program
    {
        static void Main(string[] args)
        {
            // 引用添加项目Daylily.Common
            Panda newPlugin = new Panda();
            newPlugin.OnInitialized(args);
            CoolQNavigableMessage cm = new CoolQNavigableMessage()
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
