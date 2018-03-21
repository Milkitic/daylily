using DaylilyWeb.Assist;
using DaylilyWeb.Functions.Applications;
using DaylilyWeb.Interface.CQHttp;
using DaylilyWeb.Models;
using DaylilyWeb.Models.CQRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DaylilyWeb.Functions
{
    public class MsgHandler
    {
        // 多线程相关
        static Queue<PrivateMsg> pMsgQueue = new Queue<PrivateMsg>();
        static Queue<GroupMsg> gMsgQueue = new Queue<GroupMsg>();
        static Thread gThread; // 群聊线程
        static Thread pThread; // 私聊线程

        Random rnd = new Random();
        bool gLockMsg = false, pLockMsg = false; // 用于判断是否超出消息阀值
        int gMsgLimit = 10, pMsgLimit = 4;
        int minTime = 200, maxTime = 300; // 回应的反应时间

        HttpApi CQApi = new HttpApi();

        /// <summary>
        /// 群聊消息
        /// </summary>
        public MsgHandler(GroupMsg parsed_obj)
        {
            GroupMsg currentInfo = parsed_obj;
            if (gMsgQueue.Count < gMsgLimit) // 允许缓存n条，再多的丢弃
                gMsgQueue.Enqueue(currentInfo);

            else if (!gLockMsg)
            {
                gLockMsg = true;
                string response = _sendMsg("能不能慢点啊，你们这想整死我（", null, currentInfo.group_id.ToString());
                Log.PrimaryLine(response, ToString(), "MsgHandler(GroupMsg)");
            }

            if (gThread == null || (gThread.ThreadState != ThreadState.Running && gThread.ThreadState != ThreadState.WaitSleepJoin))
            {
                gThread = new Thread(HandleGroupMessage);
                gThread.Start();
            }
        }

        /// <summary>
        /// 私聊消息
        /// </summary>
        public MsgHandler(PrivateMsg parsed_obj)
        {
            PrivateMsg currentInfo = parsed_obj;
            if (pMsgQueue.Count < pMsgLimit) // 允许缓存n条，再多的丢弃
                pMsgQueue.Enqueue(currentInfo);

            else if (!pLockMsg)
            {
                pLockMsg = true;
                string response = _sendMsg("？？求您慢点说话好吗", currentInfo.user_id.ToString());
                Log.PrimaryLine(response, ToString(), "MsgHandler(PrivateMsg)");
            }

            if (pThread == null || (pThread.ThreadState != ThreadState.Running && pThread.ThreadState != ThreadState.WaitSleepJoin))
            {
                pThread = new Thread(HandlePrivateMessage);
                pThread.Start();
            }
        }

        private void HandleGroupMessage()
        {
            while (gMsgQueue.Count != 0)
            {
                if (gMsgQueue.Count == 0) break; // 不加这条总有奇怪的错误发生

                var currentInfo = gMsgQueue.Dequeue();

                string message = currentInfo.message.Replace("\n", "").Replace("\r", "").Trim();
                string user = currentInfo.user_id.ToString();
                string group = currentInfo.group_id.ToString();

                try
                {
                    _hdleMsg(message, user, group);
                }
                catch (Exception ex)
                {
                    string response = _sendMsg(ex.Message, user, group);
                }
            }
            gLockMsg = false;
        }

        private void HandlePrivateMessage()
        {
            while (pMsgQueue.Count != 0)
            {
                if (pMsgQueue.Count == 0) break; // 不加这条总有奇怪的错误发生

                var currentInfo = pMsgQueue.Dequeue();

                string message = currentInfo.message.Replace("\n", "").Replace("\r", "").Trim();
                string user = currentInfo.user_id.ToString();

                try
                {
                    _hdleMsg(message, user, null);
                }
                catch (Exception ex)
                {
                    string response = _sendMsg(ex.Message, user);
                }
            }
            pLockMsg = false;
        }
        private void _hdleMsg(string message, string user, string group = null)
        {
            Log.InfoLine(user + "：" + message, ToString(), "_hdleMsg()");
            if (message.Substring(0, 1) == "!")
            {
                Thread.Sleep(rnd.Next(minTime, maxTime));
                if (message.IndexOf("roll ") == 1)
                {
                    string command = "!roll ";
                    string result;
                    var query = message.Substring(command.Length, message.Length - command.Length).Split(' ');
                    if (!int.TryParse(query[0], out int a))
                    {
                        result = Roll.Next().ToString();
                    }
                    else if (query.Length == 1)
                    {
                        result = Roll.Next(int.Parse(query[0])).ToString();
                    }
                    else if (query.Length == 2)
                    {
                        result = Roll.Next(int.Parse(query[0]), int.Parse(query[1])).ToString();
                    }
                    else if (query.Length == 3)
                    {
                        result = Roll.Next(int.Parse(query[0]), int.Parse(query[1]), int.Parse(query[2])).ToString();
                    }
                    else throw new ArgumentException();

                    string response = _sendMsg(result, user, group);

                    Log.PrimaryLine(response, ToString(), "_hdleMsg()");
                    Log.InfoLine("发送消息：" + result, ToString(), "_hdleMsg()");
                    return;
                }
                else if (message.IndexOf("!roll") == 0 && message.Length == "!roll".Length)
                {
                    var result = Roll.Next().ToString();
                    string response = _sendMsg(result, user, group);

                    Log.PrimaryLine(response, ToString(), "_hdleMsg()");
                    Log.InfoLine("发送消息：" + result, ToString(), "_hdleMsg()");
                    return;
                }
                else if (message.IndexOf("ping ") == -1 && message.IndexOf("ping") == 1)
                {
                    var result = "!pong";
                    string response = _sendMsg(result, user, group);

                    Log.PrimaryLine(response, ToString(), "_hdleMsg()");
                    return;
                }
                //throw new NotImplementedException("尚不支持命令" + message.Split(' ')[0]);
            }
            else
            {
                // 如果不是命令 todo
            }
        }
        private string _sendMsg(string message, string user = null, string group = null)
        {
            Thread.Sleep(message.Length * 400);
            if (group != null && user != null)
            {
                return CQApi.SendGroupMessageAsync(group, CQCode.GetAt(user) + " " + message).Result;
            }
            else if (user != null)
            {
                return CQApi.SendPrivateMessageAsync(user, message).Result;
            }
            else if (group != null)
            {
                return CQApi.SendGroupMessageAsync(group, message).Result;
            }
            else
                throw new NotImplementedException("尚未支持该消息");
        }
    }
}
