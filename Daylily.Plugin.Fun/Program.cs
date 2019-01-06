using Daylily.Bot.Message;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ.CoolQHttp.ResponseModel.Report;
using Daylily.CoolQ.Message;
using System;

namespace Daylily.Plugin.ShaDiao
{
    class Program
    {
        static void Main(string[] args)
        {
            // 引用添加项目Daylily.Common
            Panda newPlugin = new Panda();
            newPlugin.OnInitialized(args);
            CoolQRouteMessage cm = new CoolQRouteMessage()
            {
                GroupId = "123456788",
                UserId = "2241521134",
                Message = CoolQMessage.Parse("SB"),
                MessageType = MessageType.Group,
                Group = new CoolQGroupMessageApi(),
            };

            Logger.Success("收到：" + newPlugin.OnMessageReceived(new CoolQ.CoolQScopeEventArgs
            {
                RouteMessage = cm
            }).Message);
            Console.ReadKey();
        }
    }
}
