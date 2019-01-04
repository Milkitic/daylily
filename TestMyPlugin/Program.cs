using Daylily.Bot.Message;
using Daylily.Common.Utils.LoggerUtils;
using System;

namespace TestMyPlugin
{
    class Program
    {
        static void Main(string[] args)
        {
            RepeatPlugin newPlugin = new RepeatPlugin();
            newPlugin.Initialize(args);
            while (true)
            {
                var msg = Console.ReadLine();
                CoolQRouteMessage cm = new CoolQRouteMessage()
                {
                    GroupId = "123456788",
                    UserId = "2241521134",
                    RawMessage = msg,
                    MessageType = MessageType.Group,
                    Group = new GroupMsg(),
                };

                Logger.Raw("回复：" + newPlugin.Message_Received(cm).Message);
            }
        }
    }
}
