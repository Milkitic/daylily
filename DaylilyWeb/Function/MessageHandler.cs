using DaylilyWeb.Assist;
using DaylilyWeb.Function.Application;
using DaylilyWeb.Interface.CQHttp;
using DaylilyWeb.Models;
using DaylilyWeb.Models.CQResponse;
using DaylilyWeb.Models.CQResponse.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DaylilyWeb.Function
{
    public class MessageHandler
    {
        public static GroupList GroupInfo { get; set; } = new GroupList();
        public static DiscussList DiscussInfo { get; set; } = new DiscussList();
        public static PrivateList PrivateInfo { get; set; } = new PrivateList();

        public static string COMMAND_FLAG = "!";

        Random rnd = new Random();
        int minTime = 200, maxTime = 300; // 回应的反应时间
        //string UserId = null, GroupId = null, DiscussId = null;
        //MessageType messageType;

        HttpApi CQApi = new HttpApi();

        /// <summary>
        /// 群聊消息
        /// </summary>
        public MessageHandler(GroupMsg parsed_obj)
        {
            long id = parsed_obj.GroupId;

            GroupInfo.Add(id);
            if (GroupInfo[id].MsgQueue.Count < GroupInfo[id].MsgLimit) // 允许缓存n条，再多的丢弃
                GroupInfo[id].MsgQueue.Enqueue(parsed_obj);

            else if (!GroupInfo[id].LockMsg)
            {
                GroupInfo[id].LockMsg = true;
                SendMessage(parsed_obj.Message, id.ToString(), null, null, MessageType.Group, false);
            }

            if (GroupInfo[id].Thread == null ||
                (GroupInfo[id].Thread.ThreadState != ThreadState.Running && GroupInfo[id].Thread.ThreadState != ThreadState.WaitSleepJoin))
            {
                GroupInfo[id].Thread = new Thread(new ParameterizedThreadStart(HandleGroupMessage));
                GroupInfo[id].Thread.Start(parsed_obj);
            }
        }
        /// <summary>
        /// 讨论组消息
        /// </summary>
        public MessageHandler(DiscussMsg parsed_obj)
        {
            long id = parsed_obj.DiscussId;

            DiscussInfo.Add(id);
            DiscussMsg currentInfo = parsed_obj;
            if (DiscussInfo[id].MsgQueue.Count < DiscussInfo[id].MsgLimit) // 允许缓存n条，再多的丢弃
                DiscussInfo[id].MsgQueue.Enqueue(currentInfo);

            else if (!DiscussInfo[id].LockMsg)
            {
                DiscussInfo[id].LockMsg = true;
                SendMessage(parsed_obj.Message, null, null, id.ToString(), MessageType.Discuss, false);
            }

            if (DiscussInfo[id].Thread == null ||
                (DiscussInfo[id].Thread.ThreadState != ThreadState.Running && DiscussInfo[id].Thread.ThreadState != ThreadState.WaitSleepJoin))
            {
                DiscussInfo[id].Thread = new Thread(new ParameterizedThreadStart(HandleDiscussMessage));
                DiscussInfo[id].Thread.Start(parsed_obj);
            }
        }
        /// <summary>
        /// 私聊消息
        /// </summary>
        public MessageHandler(PrivateMsg parsed_obj)
        {
            long id = parsed_obj.UserId;

            PrivateInfo.Add(id);
            PrivateMsg currentInfo = parsed_obj;
            if (PrivateInfo[id].MsgQueue.Count < PrivateInfo[id].MsgLimit) // 允许缓存n条，再多的丢弃
                PrivateInfo[id].MsgQueue.Enqueue(currentInfo);

            else if (!PrivateInfo[id].LockMsg)
            {
                PrivateInfo[id].LockMsg = true;
                SendMessage("？？求您慢点说话好吗", null, id.ToString(), null, MessageType.Private, false);
            }

            if (PrivateInfo[id].Thread == null ||
                (PrivateInfo[id].Thread.ThreadState != ThreadState.Running && PrivateInfo[id].Thread.ThreadState != ThreadState.WaitSleepJoin))
            {
                PrivateInfo[id].Thread = new Thread(new ParameterizedThreadStart(HandlePrivateMessage));
                PrivateInfo[id].Thread.Start(parsed_obj);
            }
        }

        private void HandleGroupMessage(object obj)
        {
            var parsed_obj = (GroupMsg)obj;
            MessageType messageType = MessageType.Group;
            string UserId = parsed_obj.UserId.ToString(),
                GroupId = parsed_obj.GroupId.ToString();
           
            long groupId = long.Parse(GroupId);
            while (GroupInfo[groupId].MsgQueue.Count != 0)
            {
                if (GroupInfo[groupId].MsgQueue.Count == 0) break; // 不加这条总有奇怪的错误发生

                var currentInfo = GroupInfo[groupId].MsgQueue.Dequeue();

                string message = currentInfo.Message.Replace("\n", "").Replace("\r", "").Trim();

                try
                {
                    HandleMessage(message, currentInfo.GroupId.ToString(), currentInfo.UserId.ToString(), null, messageType);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        Logger.DangerLine(ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace);
                    else
                        Logger.DangerLine(ex.Message + Environment.NewLine + ex.StackTrace);
                    GC.Collect();
                }
            }
            GroupInfo[groupId].LockMsg = false;
        }
        private void HandleDiscussMessage(object obj)
        {
            var parsed_obj = (DiscussMsg)obj;
            MessageType messageType = MessageType.Discuss;
            string UserId = parsed_obj.UserId.ToString(),
                DiscussId = parsed_obj.DiscussId.ToString();
      
            long discussId = long.Parse(DiscussId);
            while (DiscussInfo[discussId].MsgQueue.Count != 0)
            {
                if (DiscussInfo[discussId].MsgQueue.Count == 0) break; // 不加这条总有奇怪的错误发生

                var currentInfo = DiscussInfo[discussId].MsgQueue.Dequeue();

                string message = currentInfo.Message.Replace("\n", "").Replace("\r", "").Trim();

                try
                {
                    HandleMessage(message, null, currentInfo.UserId.ToString(), currentInfo.DiscussId.ToString(), messageType);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        Logger.DangerLine(ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace);
                    else
                        Logger.DangerLine(ex.Message + Environment.NewLine + ex.StackTrace);
                    GC.Collect();
                }
            }
            DiscussInfo[discussId].LockMsg = false;
        }
        private void HandlePrivateMessage(object obj)
        {
            var parsed_obj = (PrivateMsg)obj;
            MessageType messageType = MessageType.Private;
            string UserId = parsed_obj.UserId.ToString();

            long userId = long.Parse(UserId);
            while (PrivateInfo[userId].MsgQueue.Count != 0)
            {
                if (PrivateInfo[userId].MsgQueue.Count == 0) break; // 不加这条总有奇怪的错误发生

                var currentInfo = PrivateInfo[userId].MsgQueue.Dequeue();

                string message = currentInfo.Message.Replace("\n", "").Replace("\r", "").Trim();

                try
                {
                    HandleMessage(message, null, currentInfo.UserId.ToString(), null, messageType);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        Logger.DangerLine(ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace);
                    else
                        Logger.DangerLine(ex.Message + Environment.NewLine + ex.StackTrace);
                    GC.Collect();
                }
            }
            PrivateInfo[userId].LockMsg = false;
        }

        private void HandleMessage(string message, string GroupId, string UserId, string DiscussId, MessageType messageType)
        {
            long groupId = Convert.ToInt64(GroupId);
            long userId = Convert.ToInt64(UserId);
            long discussId = Convert.ToInt64(DiscussId);
            if (messageType == MessageType.Private)
            {
                Logger.InfoLine($"{userId}: {message}");
            }
            else if (messageType == MessageType.Group)
            {
                var userInfo = CQApi.GetGroupMemberInfo(GroupId, UserId);  // 有点费时间
                Logger.InfoLine($"({GroupInfo[groupId].Name}) {userInfo.Data.Nickname}: {message}");
            }
            else if (messageType == MessageType.Discuss)
            {
                Logger.InfoLine($"({DiscussInfo[discussId].Name}) {userId}: {message}");
            }

            if (message.Substring(0, 1) == COMMAND_FLAG)
            {
                if (message.IndexOf(COMMAND_FLAG + "root ") == 0)
                {
                    if (UserId != "2241521134")
                    {
                        SendMessage("你没有权限...", GroupId, UserId, DiscussId, messageType, true);
                    }
                    else
                    {
                        string fullCommand = message.Substring(6, message.Length - 6);
                        HandleMessageCmd(fullCommand, GroupId, UserId, DiscussId, messageType, PermissionLevel.Root);
                    }

                }
                else if (message.IndexOf(COMMAND_FLAG + "sudo ") == 0 && messageType == MessageType.Group)
                {
                    if (!GroupInfo[groupId].AdminList.Contains(UserId))
                    {
                        SendMessage("你没有权限...仅本群管理员可用", GroupId, UserId, DiscussId, messageType, true);
                    }
                    else
                    {
                        string fullCommand = message.Substring(6, message.Length - 6);
                        HandleMessageCmd(fullCommand, GroupId, UserId, DiscussId, messageType, PermissionLevel.Admin);
                    }
                }
                else
                {
                    string fullCommand = message.Substring(1, message.Length - 1);
                    HandleMessageCmd(fullCommand, GroupId, UserId, DiscussId, messageType, PermissionLevel.Public);
                }

            }
            HandleMesasgeApp(message, GroupId, UserId, DiscussId, messageType);

        }
        private void HandleMesasgeApp(string message, string GroupId, string UserId, string DiscussId, MessageType messageType)
        {
            foreach (var item in Mapper.NormalPlugins)
            {
                string reply = null;

                #region 折叠：invoke
                Type type = Type.GetType("DaylilyWeb.Function.Application." + item);
                MethodInfo mi = type.GetMethod("Execute");
                var ok = type.GetMethods();
                object appClass = Activator.CreateInstance(type);
                object[] invokeArgs = { message, UserId, GroupId, PermissionLevel.Public, false };

                #endregion

                bool enableAt = false;
                try
                {
                    reply = (string)mi.Invoke(appClass, invokeArgs);
                    enableAt = (bool)invokeArgs[4];
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        throw new Exception("\n\"" + message + "\" caused an exception: \n" +
                            type.Name + ": " + ex.InnerException.Message + "\n\n" + ex.InnerException.StackTrace);
                    else
                        throw new Exception("\n\"" + message + "\" caused an exception: \n" +
                            type.Name + ": " + ex.Message + "\n\n" + ex.StackTrace);
                }
                if (reply == null) continue;
                SendMessage(reply, GroupId, UserId, DiscussId, messageType, enableAt);
            }
        }
        private void HandleMessageCmd(string fullCommand, string GroupId, string UserId, string DiscussId, MessageType messageType, PermissionLevel currentLevel)
        {
            Thread.Sleep(rnd.Next(minTime, maxTime));
            bool enableAt = false;

            string command = fullCommand.Split(' ')[0].Trim();
            string param = fullCommand.IndexOf(" ") == -1 ? "" : fullCommand.Substring(fullCommand.IndexOf(" ") + 1, fullCommand.Length - command.Length - 1).Trim();
            string className = Mapper.GetClassName(command, out string file);
            if (className == null)
                return;

            MethodInfo mi;
            object appClass;
            Type type;
            System.IO.FileInfo fi = null;
            if (file == null)
            {
                type = Type.GetType("DaylilyWeb.Function.Application.Command." + className);
                appClass = Activator.CreateInstance(type);
            }
            else
            {
                try
                {
                    Logger.PrimaryLine("读取插件信息中");
                    fi = new System.IO.FileInfo(file);
                    Assembly assemblyTmp = Assembly.LoadFrom(file);
                    type = assemblyTmp.GetType(className);
                    appClass = assemblyTmp.CreateInstance(className);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        throw new Exception("\n/\"" + fullCommand + "\" caused an exception: \n" +
                            fi.Name + ": " + ex.InnerException.Message + "\n\n" + ex.InnerException.StackTrace);
                    else
                        throw new Exception("\n/\"" + fullCommand + "\" caused an exception: \n" +
                            fi.Name + ": " + ex.Message + "\n\n" + ex.StackTrace);
                }
            }

            object[] invokeArgs = { param, UserId, GroupId, currentLevel, false };
            string reply = null;
            try
            {
                mi = type.GetMethod("Execute");
                reply = (string)mi.Invoke(appClass, invokeArgs);
                enableAt = (bool)invokeArgs[4];
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw new Exception("\n/\"" + fullCommand + "\" caused an exception: \n" +
                        type.Name + ": " + ex.InnerException.Message + "\n\n" + ex.InnerException.StackTrace);
                else
                    throw new Exception("\n/\"" + fullCommand + "\" caused an exception: \n" +
                        type.Name + ": " + ex.Message + "\n\n" + ex.StackTrace);
            }

            if (reply == null) return;
            SendMessage(reply, GroupId, UserId, DiscussId, messageType, enableAt);
        }

        private void SendMessage(string message, string GroupId, string UserId, string DiscussId, MessageType messageType, bool enableAt)
        {
            if (messageType == MessageType.Group)
            {
                SendGroupMsgResponse msg = CQApi.SendGroupMessageAsync(GroupId, (enableAt ? CQCode.EncodeAt(UserId) + " " : "") + message).Result;
                Logger.InfoLine($"我: {message} {{status: {msg.Status}}})");
            }
            else if (messageType == MessageType.Discuss)
            {
                SendDiscussMsgResponse msg = CQApi.SendDiscussMessageAsync(DiscussId, (enableAt ? CQCode.EncodeAt(UserId) + " " : "") + message).Result;
                Logger.InfoLine($"我: {message} {{status: {msg.Status}}})");
            }
            else if (messageType == MessageType.Private)
            {
                SendPrivateMsgResponse msg = CQApi.SendPrivateMessageAsync(UserId, message).Result;
                Logger.InfoLine($"我: {message} {{status: {msg.Status}}})");
            }
        }
    }
}
