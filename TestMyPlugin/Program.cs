using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ.Models.CqResponse;
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
                CommonMessage cm = new CommonMessage()
                {
                    GroupId = "123456788",
                    UserId = "2241521134",
                    Message = msg,
                    MessageType = MessageType.Group,
                    Group = new GroupMsg(),
                };

                Logger.Raw("回复：" + newPlugin.Message_Received(cm).Message);
            }

            Console.ReadKey();
        }
    }

    [Name("复读")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Beta)]
    [Help("复读。")]
    [Command("repeat")]
    public class RepeatPlugin : CommandPlugin
    {
        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            return new CommonMessageResponse(messageObj.Message, messageObj);
        }
    }
}
