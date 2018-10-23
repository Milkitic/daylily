using System;
using System.Linq;
using CSharpOsu.Module;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ.Models.CqResponse;
using Daylily.Osu.Interface;

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
            newPlugin.Initialize(args);
            CommonMessage cm = new CommonMessage
            {
                GroupId = "123456788",
                UserId = "2241521134",
                Message = "SB",
                MessageType = MessageType.Private,
                Group = new GroupMsg(),
                Discuss = new DiscussMsg(),
                Private = new PrivateMsg(),
            };

            Logger.Success("收到：" + newPlugin.Message_Received(cm).Message);
            Console.ReadKey();
        }
    }
}
