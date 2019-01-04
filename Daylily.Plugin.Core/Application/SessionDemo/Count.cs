using Daylily.Bot.Message;
using System;
using Daylily.Bot.Backend;
using Daylily.Bot.Session;
using Daylily.CoolQ.Message;

namespace Daylily.Plugin.Core.Application.SessionDemo
{
    [Name("数羊")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Stable)]
    [Help("数羊测试。Session应用的demo。")]
    internal class Count : ApplicationPlugin
    {

        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMsg)
        {
            if (!routeMsg.RawMessage.Contains("数咩羊"))
                return null;

            using (Session session = new Session(8000.CqIdentity.UserId))
            {
                SendMessage(routeMsg.ToSource("睡不着那就一起数咩羊吧。来，我先开始，1！"));
                int count = 1;
                try
                {
                    CoolQRouteMessage obj = session.GetMessage();
                    do
                    {
                        System.Threading.Thread.Sleep(1000);

                        if (int.TryParse(obj.RawMessage, out var res))
                        {
                            if (res == count + 1)
                            {
                                count += 2;
                                SendMessage(routeMsg.ToSource(count.ToString()));
                            }
                            else
                            {
                                SendMessage(routeMsg.ToSource($"你数错啦，是不是困了？现在是{count}，到你了"));
                            }
                        }
                        else
                            SendMessage(routeMsg.ToSource($"不对！要好好数数哦！现在是{count}，到你了"));

                        System.Threading.Thread.Sleep(1000);

                        if (count > 15)
                        {
                            SendMessage(routeMsg.ToSource("不数羊了，人家都困了，聊点别的吧！"));
                            break;
                        }
                        obj = session.GetMessage();
                    } while (obj != null);
                }
                catch (TimeoutException)
                {
                    SendMessage(routeMsg.ToSource("数不动了吗，那就好好睡觉咯，晚安！"));
                }

            }

            return null;
        }
    }
}
