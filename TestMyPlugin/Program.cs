using Daylily.Bot.Message;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ.Models.CqResponse;
using System;
using Daylily.CoolQ.Message;

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
                CoolQNavigableMessage cm = new CoolQNavigableMessage()
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
