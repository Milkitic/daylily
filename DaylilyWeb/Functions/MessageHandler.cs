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
    public class MessageHandler
    {
        public static GroupList GroupInfo { get; set; } = new GroupList();
        public static PrivateList PrivateInfo { get; set; } = new PrivateList();

        Random rnd = new Random();
        int minTime = 200, maxTime = 300; // 回应的反应时间

        HttpApi CQApi = new HttpApi();

        /// <summary>
        /// 群聊消息
        /// </summary>
        public MessageHandler(GroupMsg parsed_obj)
        {
            long id = parsed_obj.group_id;
            GroupInfo.Add(id);
            GroupMsg currentInfo = parsed_obj;
            if (GroupInfo[id].MsgQueue.Count < GroupInfo[id].MsgLimit) // 允许缓存n条，再多的丢弃
                GroupInfo[id].MsgQueue.Enqueue(currentInfo);

            else if (!GroupInfo[id].LockMsg)
            {
                GroupInfo[id].LockMsg = true;
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
        public MessageHandler(PrivateMsg parsed_obj)
        {
            long id = parsed_obj.user_id;
            PrivateInfo.Add(id);
            PrivateMsg currentInfo = parsed_obj;
            if (PrivateInfo[id].MsgQueue.Count < PrivateInfo[id].MsgLimit) // 允许缓存n条，再多的丢弃
                PrivateInfo[id].MsgQueue.Enqueue(currentInfo);

            else if (!PrivateInfo[id].LockMsg)
            {
                PrivateInfo[id].LockMsg = true;
                string response = _sendMsg("？？求您慢点说话好吗", currentInfo.user_id.ToString());
                Log.PrimaryLine(response, ToString(), "MsgHandler(PrivateMsg)");
            }

            if (PrivateInfo[id].Thread == null ||
                (PrivateInfo[id].Thread.ThreadState != ThreadState.Running && PrivateInfo[id].Thread.ThreadState != ThreadState.WaitSleepJoin))
            {
                PrivateInfo[id].Thread = new Thread(new ParameterizedThreadStart(HandlePrivateMessage));
                PrivateInfo[id].Thread.Start(id);
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
                    _HandleMessage(message, user, group);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        _sendMsg(ex.InnerException.Message, user, group);
                    else
                        _sendMsg(ex.Message, user, group);
                    GC.Collect();
                }
                GroupInfo[groupId].PreInfo = currentInfo;
            }
            GroupInfo[groupId].LockMsg = false;
        }
        private void HandlePrivateMessage(object param)
        {
            long privateId = (long)param;
            while (PrivateInfo[privateId].MsgQueue.Count != 0)
            {
                if (PrivateInfo[privateId].MsgQueue.Count == 0) break; // 不加这条总有奇怪的错误发生

                var currentInfo = PrivateInfo[privateId].MsgQueue.Dequeue();

                string message = currentInfo.message.Replace("\n", "").Replace("\r", "").Trim();
                string user = currentInfo.user_id.ToString();

                try
                {
                    _HandleMessage(message, user, null);
                }
                catch (System.Net.WebException ex)
                {
                    // _sendMsg(WebRequestHelper.GetResponseString((System.Net.HttpWebResponse)ex.Response), user);
                }
                catch (Exception ex)
                {
                    //Log.DangerLine(ex.Message);
                    if (ex.InnerException != null)
                        Log.DangerLine(ex.InnerException.Message);
                    //_sendMsg(ex.InnerException.Message, user);
                    else
                        Log.DangerLine(ex.Message);
                    //_sendMsg(ex.Message, user);
                    GC.Collect();
                }
            }
            PrivateInfo[privateId].LockMsg = false;
        }

        private void _HandleMessage(string message, string user, string group = null)
        {
            long groupId = Convert.ToInt64(group);
            bool isGroup = group != null;
            Log.InfoLine(user + "：" + message, ToString(), "_HandleMessage()");
            if (message.Substring(0, 1) == "!")
            {
                if (message.IndexOf("!sudo ") == 0)
                {
                    if (user != "2241521134") return;
                    string fullCommand = message.Substring(6, message.Length - 6);
                    _HandleMessageCmd(fullCommand, user, isGroup, true, group);
                }
                else
                {
                    string fullCommand = message.Substring(1, message.Length - 1);
                    _HandleMessageCmd(fullCommand, user, isGroup, false, group);
                }

            }

            //if (isGroup)
            //{
            //    if (GroupInfo[groupId].PreInfo.message == message && message != GroupInfo[groupId].PreString)
            //    {
            //        string response = _sendMsg(message, null, group);
            //        Log.PrimaryLine(response, ToString(), "_hdleMsg()");
            //        GroupInfo[groupId].PreString = message;
            //        return;
            //    }
            //}
            _HandleMesasgeApp(message, user, isGroup, group);

        }
        private void _HandleMesasgeApp(string message, string user, bool isGroup, string group)
        {
            foreach (var item in Mapper.NormalPlugins)
            {
                string reply = null;

                #region 折叠：invoke
                Type type = Type.GetType("DaylilyWeb.Functions.Applications." + item);
                MethodInfo mi = type.GetMethod("Execute");
                var ok = type.GetMethods();
                object appClass = Activator.CreateInstance(type);
                object[] invokeArgs = { message, user, group, false, false };

                #endregion

                bool ifAt = false;
                try
                {
                    reply = (string)mi.Invoke(appClass, invokeArgs);
                    ifAt = (bool)invokeArgs[4];
                }
                catch (TargetParameterCountException ex)
                {
                    throw new Exception("\"" + message + "\" caused an exception: \r\n" + type.Name.ToLower() + ": " + ex.Message);
                }
                if (reply == null) continue;
                string response = isGroup ? _sendMsg(reply, user, group, ifAt) : _sendMsg(reply, user, null);
                Log.PrimaryLine(response, ToString(), "_HandleMesasgeApp()");
            }
        }

        private void _HandleMessageCmd(string fullCommand, string user, bool isGroup, bool isRoot, string group)
        {
            Thread.Sleep(rnd.Next(minTime, maxTime));
            group = null;
            bool enableAt = false;

            string command = fullCommand.Split(' ')[0];
            string param = fullCommand.IndexOf(" ") == -1 ? "" : fullCommand.Substring(fullCommand.IndexOf(" ") + 1, fullCommand.Length - command.Length - 1);
            string className = Mapper.GetClassName(command, out string file);
            if (className == null)
                return;

            MethodInfo mi;
            object appClass;
            Type type;
            System.IO.FileInfo fi = null;
            if (file == null)
            {
                type = Type.GetType("DaylilyWeb.Functions.Applications." + className);
                appClass = Activator.CreateInstance(type);
            }
            else
            {
                try
                {
                    Log.PrimaryLine("读取插件信息中", ToString(), "_hdleMsg()");
                    fi = new System.IO.FileInfo(file);
                    Assembly assemblyTmp = Assembly.LoadFrom(file);
                    type = assemblyTmp.GetType(className);
                    appClass = assemblyTmp.CreateInstance(className);
                }
                catch (Exception)
                {
                    throw new Exception("\"" + fi.Name + "\" 这东西根本就是假的，是带特技的");
                }
            }

            object[] invokeArgs = { param, user, group, isRoot, false };
            string result = null;
            try
            {
                mi = type.GetMethod("Execute");
                result = (string)mi.Invoke(appClass, invokeArgs);
                enableAt = (bool)invokeArgs[4];
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw new Exception("\"" + fullCommand + "\" caused an exception: \r\n" + type.Name + ": " + ex.InnerException.Message);
                else
                    throw new Exception("\"" + fullCommand + "\" caused an exception: \r\n" + type.Name + ": " + ex.Message);
            }

            string response = isGroup ? _sendMsg(result, user, group, enableAt) : _sendMsg(result, user, null);
            Log.PrimaryLine(response, ToString(), "_HandleMessageCmd()");
        }

        private string _sendMsg(string message, string user = null, string group = null, bool enableAt = false)
        {
            if (group != null && user != null)
            {
                return CQApi.SendGroupMessageAsync(group, (enableAt ? CQCode.EncodeAt(user) + " " : "") + message).Result;
                //return CQApi.SendGroupMessageAsync(group, CQCode.EncodeAt(user) + " " + message).Result;
            }
            else if (user != null)
            {
                return CQApi.SendPrivateMessageAsync(user, message).Result;
            }
            else
            {
                return CQApi.SendGroupMessageAsync(group, message).Result;
            }
        }
    }
}
