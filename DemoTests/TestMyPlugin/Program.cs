using Daylily.Bot;
using Daylily.Common.Logging;
using Daylily.CoolQ.CoolQHttp.ResponseModel.Report;
using Daylily.CoolQ.Messaging;
using Daylily.Plugin.Basic;
using System;
using Daylily.Bot.Messaging;

namespace TestMyPlugin
{
    class Program
    {
        static void Main(string[] args)
        {
            Roll newPlugin = new Roll();
            newPlugin.OnInitialized(new StartupConfig(null, null, new StartupConfig.Metadata()));
            while (true)
            {
                var msg = Console.ReadLine();
                CoolQRouteMessage cm = CoolQRouteMessage.Parse(new CoolQGroupMessageApi
                {
                    GroupId = 123456788,
                    UserId = 2241521134,
                    Message = msg,
                });

                Logger.Raw("回复：" + newPlugin.OnMessageReceived(new Daylily.CoolQ.CoolQScopeEventArgs
                {
                    RouteMessage = cm
                }).RawMessage);
            }
        }
    }
}
