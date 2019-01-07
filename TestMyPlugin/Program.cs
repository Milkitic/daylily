using Daylily.Bot.Message;
using Daylily.Common.Logging;
using Daylily.CoolQ.CoolQHttp.ResponseModel.Report;
using Daylily.CoolQ.Message;
using Daylily.Plugin.Basic;
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
                    Message = CoolQMessage.Parse(msg),
                    MessageType = MessageType.Group,
                    Group = new CoolQGroupMessageApi(),
                };

                Logger.Raw("回复：" + newPlugin.OnMessageReceived(new Daylily.CoolQ.CoolQScopeEventArgs
                {
                    RouteMessage = cm
                }).RawMessage);
            }
        }
    }
}
