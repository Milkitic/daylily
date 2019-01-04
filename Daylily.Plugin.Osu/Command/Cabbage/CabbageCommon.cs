using Daylily.Bot;
using Daylily.Bot.Enum;
using Daylily.Bot.Message;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.Common.Utils.StringUtils;
using Daylily.CoolQ.Models;
using Daylily.Osu.Database.BLL;
using Daylily.Osu.Database.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Daylily.Bot.Session;
using Daylily.CoolQ.Message;

namespace Daylily.Plugin.Osu.Cabbage
{
    internal static class CabbageCommon
    {
        public static readonly ConcurrentQueue<CoolQRouteMessage> MessageQueue = new ConcurrentQueue<CoolQRouteMessage>();
        public static Task TaskQuery;

        public static void Query()
        {
            while (MessageQueue.Count != 0)
            {
                if (!MessageQueue.TryDequeue(out var messageObj))
                    continue;
                var cmd = messageObj.Command;

                const long cabbageId = 1020640876;
                string uname;
                if (cmd == "statme" || cmd == "bpme" || cmd == "mybp" || cmd == "costme" || cmd == "mycost")
                {
                    BllUserRole bllUserRole = new BllUserRole();
                    List<TblUserRole> userInfo = bllUserRole.GetUserRoleByQq(long.Parse(messageObj.UserId));
                    if (userInfo.Count == 0)
                        CoolQDispatcher.Current.SendMessage(routeMsg.ToSource(LoliReply.IdNotBound, messageObj, true));

                    uname = userInfo[0].CurrentUname;
                }
                else
                    uname = messageObj.ArgString;

                using (Session session = new Session(25000, new CqIdentity(cabbageId, MessageType.Private), cabbageId))
                {
                    CoolQDispatcher.Current.SendMessage(
                        routeMsg.ToSource($"!{cmd.Replace("my", "").Replace("me", "")} {uname}",
                            new CqIdentity(cabbageId, MessageType.Private)));
                    try
                    {
                        CoolQRouteMessage result = session.GetMessage();
                        session.Timeout = 600;
                        CoolQRouteMessage result2 = null;
                        try
                        {
                            result2 = session.GetMessage();
                        }
                        catch
                        {
                            // ignored
                        }

                        ImageInfo[] imgList =
                            CoolQCode.GetImageInfo(result. RawMessage) ?? CoolQCode.GetImageInfo(result2?.RawMessage);

                        if (imgList == null)
                        {
                            CoolQDispatcher.Current.SendMessage(routeMsg.ToSource(result.RawMessage, messageObj));
                            if (result2 != null)
                                CoolQDispatcher.Current.SendMessage(routeMsg.ToSource(result2.RawMessage, messageObj));
                            continue;
                        }
                        //throw new IndexOutOfRangeException("查询失败：" + result.Message);
                        var message = CoolQCode.DecodeToString(result.RawMessage);
                        foreach (var item in imgList)
                        {
                            var str = new FileImage(new Uri(item.Url));
                            StringFinder sf = new StringFinder(message);
                            sf.FindNext("[图片]");
                            string str1 = sf.Cut();
                            if (sf.FindNext("[图片]", false) > message.Length - 1)
                            {
                                message = str1 + str;
                                continue;
                            }

                            sf.FindToLast();
                            string str2 = sf.Cut();
                            message = str1 + str + str2;
                        }

                        CoolQDispatcher.Current.SendMessage(
                            routeMsg.ToSource(message + "\r\n（查询由白菜支持）", messageObj));
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        string msg = e.Message;
                        CoolQDispatcher.Current.SendMessage(routeMsg.ToSource(msg, messageObj, true));
                    }
                    catch (TimeoutException)
                    {
                        string msg = "查询失败，白菜没有搭理人家..";
                        CoolQDispatcher.Current.SendMessage(routeMsg.ToSource(msg, messageObj, true));
                    }
                    catch (Exception ex)
                    {
                        string msg = "查询失败，未知错误。";
                        Logger.Exception(ex);
                        CoolQDispatcher.Current.SendMessage(routeMsg.ToSource(msg, messageObj, true));
                    } // catch
                } // using
            } // while
        } //void
    }
}
