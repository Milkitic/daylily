using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Daylily.Bot;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.Sessions;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.Common.Utils.StringUtils;
using Daylily.CoolQ;
using Daylily.CoolQ.Interface.CqHttp;
using Daylily.CoolQ.Models;
using Daylily.Osu.Database.BLL;
using Daylily.Osu.Database.Model;

namespace Daylily.Plugin.Osu.Cabbage
{
    internal static class CabbageCommon
    {
        public static readonly ConcurrentQueue<CommonMessage> MessageQueue = new ConcurrentQueue<CommonMessage>();
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
                        CoolQDispatcher.SendMessage(new CommonMessageResponse(LoliReply.IdNotBound, messageObj, true));

                    uname = userInfo[0].CurrentUname;
                }
                else
                    uname = messageObj.ArgString;

                using (Session session = new Session(25000, new Identity(cabbageId, MessageType.Private), cabbageId))
                {
                    CoolQDispatcher.SendMessage(
                        new CommonMessageResponse($"!{cmd.Replace("my", "").Replace("me", "")} {uname}",
                            new Identity(cabbageId, MessageType.Private)));
                    try
                    {
                        CommonMessage result = session.GetMessage();
                        session.Timeout = 600;
                        CommonMessage result2 = null;
                        try
                        {
                            result2 = session.GetMessage();
                        }
                        catch
                        {
                            // ignored
                        }

                        ImageInfo[] imgList =
                            CqCode.GetImageInfo(result.Message) ?? CqCode.GetImageInfo(result2?.Message);

                        if (imgList == null)
                        {
                            CoolQDispatcher.SendMessage(new CommonMessageResponse(result.Message, messageObj));
                            if (result2 != null)
                                CoolQDispatcher.SendMessage(new CommonMessageResponse(result2.Message, messageObj));
                            continue;
                        }
                        //throw new IndexOutOfRangeException("查询失败：" + result.Message);
                        var message = CqCode.DecodeToString(result.Message);
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

                        CoolQDispatcher.SendMessage(
                            new CommonMessageResponse(message + "\r\n（查询由白菜支持）", messageObj));
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        string msg = e.Message;
                        CoolQDispatcher.SendMessage(new CommonMessageResponse(msg, messageObj, true));
                    }
                    catch (TimeoutException)
                    {
                        string msg = "查询失败，白菜没有搭理人家..";
                        CoolQDispatcher.SendMessage(new CommonMessageResponse(msg, messageObj, true));
                    }
                    catch (Exception ex)
                    {
                        string msg = "查询失败，未知错误。";
                        Logger.Exception(ex);
                        CoolQDispatcher.SendMessage(new CommonMessageResponse(msg, messageObj, true));
                    } // catch
                } // using
            } // while
        } //void
    }
}
