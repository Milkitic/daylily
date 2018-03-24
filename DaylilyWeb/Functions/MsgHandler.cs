using DaylilyWeb.Assist;
using DaylilyWeb.Functions.Applications;
using DaylilyWeb.Interface.CQHttp;
using DaylilyWeb.Models;
using DaylilyWeb.Models.CQRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DaylilyWeb.Functions
{
    public class MsgHandler
    {
        public static GroupList GroupInfo { get; set; } = new GroupList();
        // 多线程相关
        static Queue<PrivateMsg> pMsgQueue = new Queue<PrivateMsg>();
        //static Queue<GroupMsg> gMsgQueue = new Queue<GroupMsg>();
        //static Thread gThread; // 群聊线程
        static Thread pThread; // 私聊线程

        //static GroupMsg preInfo = new GroupMsg();
        //static string preString = "";

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
            long id = parsed_obj.group_id;
            GroupInfo.Add(id);
            GroupMsg currentInfo = parsed_obj;
            if (GroupInfo[id].MsgQueue.Count < gMsgLimit) // 允许缓存n条，再多的丢弃
                GroupInfo[id].MsgQueue.Enqueue(currentInfo);

            else if (!gLockMsg)
            {
                gLockMsg = true;
                string response = _sendMsg("能不能慢点啊，你们这想整死我（", null, currentInfo.group_id.ToString());
                Log.PrimaryLine(response, ToString(), "MsgHandler(GroupMsg)");
            }

            if (GroupInfo[id].Thread == null ||
                (GroupInfo[id].Thread.ThreadState != ThreadState.Running && GroupInfo[id].Thread.ThreadState != ThreadState.WaitSleepJoin))
            {
                GroupInfo[id].Thread = new Thread(new ParameterizedThreadStart(HandleGroupMessage));
                GroupInfo[id].Thread.Start(id);
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

        private void HandleGroupMessage(object param)
        {
            long groupId = (long)param;
            while (GroupInfo[groupId].MsgQueue.Count != 0)
            {
                if (GroupInfo[groupId].MsgQueue.Count == 0) break; // 不加这条总有奇怪的错误发生

                var currentInfo = GroupInfo[groupId].MsgQueue.Dequeue();

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
                GroupInfo[groupId].preInfo = currentInfo;
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
                    if (ex.InnerException != null)
                        _sendMsg(ex.InnerException.Message, user);
                    else
                        _sendMsg(ex.Message, user);
                }
            }
            pLockMsg = false;
        }
        private void _hdleMsg(string message, string user, string group = null)
        {
            long groupId = Convert.ToInt64(group);
            bool isGroup = group != null;
            Log.InfoLine(user + "：" + message, ToString(), "_hdleMsg()");
            if (message.Substring(0, 1) == "!")
            {
                Thread.Sleep(rnd.Next(minTime, maxTime));

                string fullCmd = message.Substring(1, message.Length - 1);
                string cmd = fullCmd.Split(' ')[0];
                string param = fullCmd.IndexOf(" ") == -1 ? "" : fullCmd.Substring(fullCmd.IndexOf(" ") + 1, fullCmd.Length - cmd.Length - 1);
                string mCmd = Mapper.GetClassName(cmd, out string file);
                if (mCmd == null)
                    return;
                //throw new NotImplementedException("尚不支持命令：" + cmd);

                MethodInfo mi;
                object appClass;
                Type type;
                System.IO.FileInfo fi = null;
                if (file == null)
                {
                    type = Type.GetType("DaylilyWeb.Functions.Applications." + mCmd);
                    mi = type.GetMethod("Execute");
                    var ok = type.GetMethods();
                    appClass = Activator.CreateInstance(type);
                }
                else
                {
                    try
                    {
                        fi = new System.IO.FileInfo(file);
                        Assembly assemblyTmp = Assembly.LoadFrom(file);
                        type = assemblyTmp.GetType(mCmd);
                        mi = type.GetMethod("Execute");
                        appClass = assemblyTmp.CreateInstance(mCmd);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("\"" + fi.Name + "\" 这东西根本就是假的，是带特技的");
                    }
                }

                object[] objParams = new object[4]; //=null
                objParams[0] = param;
                objParams[1] = user;
                objParams[2] = group;
                objParams[3] = false;
                string result = null;
                try
                {
                    result = (string)mi.Invoke(appClass, objParams);
                }
                catch (TargetParameterCountException ex)
                {
                    throw new Exception(type.Name.ToLower() + "即使是死了，钉在棺材里了，也要在墓里，用这腐朽的声带喊出\"" + ex.Message + "\"");
                }
                if (result == null)
                    return;
                string response;
                if (isGroup)
                    response = _sendMsg(result, user, group);
                else
                    response = _sendMsg(result, user, null);
                Log.PrimaryLine(response, ToString(), "_hdleMsg()");
            }
            else
            {
                if (isGroup)
                {
                    if (GroupInfo[groupId].preInfo.message == message && message != GroupInfo[groupId].preString)
                    {
                        string response = _sendMsg(message, null, group);
                        Log.PrimaryLine(response, ToString(), "_hdleMsg()");
                        GroupInfo[groupId].preString = message;
                        return;
                    }
                }
                // 如果不是命令 todo
            }
        }
        private string _sendMsg(string message, string user = null, string group = null)
        {
            Thread.Sleep(message.Length * 100);
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
