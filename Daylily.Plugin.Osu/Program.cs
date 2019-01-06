using CSharpOsu.Module;
using Daylily.Bot.Message;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ.CoolQHttp.ResponseModel.Report;
using Daylily.CoolQ.Message;
using Daylily.Osu.Interface;
using System;
using System.Linq;

namespace Daylily.Plugin.Osu
{
    class Program
    {
        static void Main(string[] args)
        {
            Subscribe newPlugin = new Subscribe
            {
                //UnsubscribeMapper = "pw384",
                //SubscribeMapper = "rustbell",
                //SubscribeMapper = "UnitedWeSin",
                SubscribeMapper = "yf_bmp",
                //SubscribeMapper = "pw384",
                //List = true
            };
            newPlugin.OnInitialized(args);
            CoolQRouteMessage cm = new CoolQRouteMessage
            {
                GroupId = "123456788",
                UserId = "2241521134",
                Message = CoolQMessage.Parse("SB"),
                MessageType = MessageType.Private,
                Group = new CoolQGroupMessageApi(),
                Discuss = new CoolQDiscussMessageApi(),
                Private = new CoolQPrivateMessageApi(),
            };

            Logger.Success("收到：" + newPlugin.OnMessageReceived(new CoolQ.CoolQScopeEventArgs
            {
                RouteMessage = cm
            }).Message);
            Console.ReadKey();
        }
    }
}
