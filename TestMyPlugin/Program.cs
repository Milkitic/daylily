using Daylily.Bot.Message;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Models.CqResponse;
using Daylily.Plugin.Core;
using System;

namespace TestMyPlugin
{
    class Program
    {
        static void Main(string[] args)
        {
            Roll newPlugin = new Roll();
            newPlugin.OnInitialized(args);
            while (true)
            {
                var msg = Console.ReadLine();
                CoolQRouteMessage cm = new CoolQRouteMessage()
                {
                    GroupId = "123456788",
                    UserId = "2241521134",
                    Message = new CoolQMessage
                    {
                        RawMessage = msg
                    },
                    MessageType = MessageType.Group,
                    Group = new CoolQGroupMessageApi(),
                };

                Logger.Raw("回复：" + newPlugin.OnMessageReceived(cm).RawMessage);
            }
        }
    }
}
