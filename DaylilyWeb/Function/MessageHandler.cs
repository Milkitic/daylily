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

        Random rnd = new Random();
        int minTime = 200, maxTime = 300; // 回应的反应时间
        string userId = null, groupId = null, discussId = null;
        MessageType messageType;

        HttpApi CQApi = new HttpApi();

        /// <summary>
        /// 群聊消息
        /// </summary>
        public MessageHandler(GroupMsg parsed_obj)
        {
            messageType = MessageType.Group;
            userId = parsed_obj.UserId.ToString();
            groupId = parsed_obj.GroupId.ToString();

            long id = parsed_obj.GroupId;

            GroupInfo.Add(id);
            GroupMsg currentInfo = parsed_obj;
            if (GroupInfo[id].MsgQueue.Count < GroupInfo[id].MsgLimit) // 允许缓存n条，再多的丢弃
                GroupInfo[id].MsgQueue.Enqueue(currentInfo);

            else if (!GroupInfo[id].LockMsg)
            {
                GroupInfo[id].LockMsg = true;
                SendMessage(parsed_obj.Message, false);
            }

            if (GroupInfo[id].Thread == null ||
                (GroupInfo[id].Thread.ThreadState != ThreadState.Running && GroupInfo[id].Thread.ThreadState != ThreadState.WaitSleepJoin))
            {
                GroupInfo[id].Thread = new Thread(HandleGroupMessage);
                GroupInfo[id].Thread.Start();
            }
        }

        /// <summary>
        /// 讨论组消息
        /// </summary>
        public MessageHandler(DiscussMsg parsed_obj)
        {
            messageType = MessageType.Discuss;
            userId = parsed_obj.UserId.ToString();
            discussId = parsed_obj.DiscussId.ToString();

            long id = parsed_obj.DiscussId;

            DiscussInfo.Add(id);
            DiscussMsg currentInfo = parsed_obj;
            if (DiscussInfo[id].MsgQueue.Count < DiscussInfo[id].MsgLimit) // 允许缓存n条，再多的丢弃
                DiscussInfo[id].MsgQueue.Enqueue(currentInfo);

            else if (!DiscussInfo[id].LockMsg)
            {
                DiscussInfo[id].LockMsg = true;
                SendMessage(parsed_obj.Message, false);
            }

            if (DiscussInfo[id].Thread == null ||
                (DiscussInfo[id].Thread.ThreadState != ThreadState.Running && DiscussInfo[id].Thread.ThreadState != ThreadState.WaitSleepJoin))
            {
                DiscussInfo[id].Thread = new Thread(HandleDiscussMessage);
                DiscussInfo[id].Thread.Start();
            }
        }

        /// <summary>
        /// 私聊消息
        /// </summary>
        public MessageHandler(PrivateMsg parsed_obj)
        {
            messageType = MessageType.Private;
            userId = parsed_obj.UserId.ToString();

            long id = parsed_obj.UserId;

            PrivateInfo.Add(id);
            PrivateMsg currentInfo = parsed_obj;
            if (PrivateInfo[id].MsgQueue.Count < PrivateInfo[id].MsgLimit) // 允许缓存n条，再多的丢弃
                PrivateInfo[id].MsgQueue.Enqueue(currentInfo);

            else if (!PrivateInfo[id].LockMsg)
            {
                PrivateInfo[id].LockMsg = true;
                SendMessage("？？求您慢点说话好吗", false);
            }

            if (PrivateInfo[id].Thread == null ||
                (PrivateInfo[id].Thread.ThreadState != ThreadState.Running && PrivateInfo[id].Thread.ThreadState != ThreadState.WaitSleepJoin))
            {
                PrivateInfo[id].Thread = new Thread(HandlePrivateMessage);
                PrivateInfo[id].Thread.Start();
            }
        }

        private void HandleGroupMessage()
        {
            long groupId = long.Parse(this.groupId);
            while (GroupInfo[groupId].MsgQueue.Count != 0)
            {
                if (GroupInfo[groupId].MsgQueue.Count == 0) break; // 不加这条总有奇怪的错误发生

                var currentInfo = GroupInfo[groupId].MsgQueue.Dequeue();

                string message = currentInfo.Message.Replace("\n", "").Replace("\r", "").Trim();

                try
                {
                    HandleMessage(message);
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
        private void HandleDiscussMessage()
        {
            long discussId = long.Parse(this.discussId);
            while (DiscussInfo[discussId].MsgQueue.Count != 0)
            {
                if (DiscussInfo[discussId].MsgQueue.Count == 0) break; // 不加这条总有奇怪的错误发生

                var currentInfo = DiscussInfo[discussId].MsgQueue.Dequeue();

                string message = currentInfo.Message.Replace("\n", "").Replace("\r", "").Trim();

                try
                {
                    HandleMessage(message);
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
        private void HandlePrivateMessage()
        {
            long userId = long.Parse(this.userId);
            while (PrivateInfo[userId].MsgQueue.Count != 0)
            {
                if (PrivateInfo[userId].MsgQueue.Count == 0) break; // 不加这条总有奇怪的错误发生

                var currentInfo = PrivateInfo[userId].MsgQueue.Dequeue();

                string message = currentInfo.Message.Replace("\n", "").Replace("\r", "").Trim();

                try
                {
                    HandleMessage(message);
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

        private void HandleMessage(string message)
        {
            long groupId = Convert.ToInt64(this.groupId);
            long userId = Convert.ToInt64(this.userId);
            long discussId = Convert.ToInt64(this.discussId);
            if (messageType == MessageType.Private)
            {
                Logger.InfoLine($"{userId}: {message}");
            }
            else if (messageType == MessageType.Group)
            {
                var userInfo = CQApi.GetGroupMemberInfo(this.groupId, this.userId);
                Logger.InfoLine($"({GroupInfo[groupId].Name}) {userInfo.Data.Nickname}: {message}");
            }
            else if (messageType == MessageType.Discuss)
            {
                var userInfo = CQApi.GetGroupMemberInfo(this.discussId, this.userId);
                Logger.InfoLine($"({DiscussInfo[discussId].Name}) {userId}: {message}");
            }

            if (message.Substring(0, 1) == "!")
            {
                if (message.IndexOf("!root ") == 0)
                {
                    if (this.userId != "2241521134")
                    {
                        SendMessage("你没有权限...", true);
                    }
                    else
                    {
                        string fullCommand = message.Substring(6, message.Length - 6);
                        HandleMessageCmd(fullCommand, PermissionLevel.Root);
                    }

                }
                else if (message.IndexOf("!sudo ") == 0 && messageType == MessageType.Group)
                {
                    if (!GroupInfo[groupId].AdminList.Contains(this.userId))
                    {
                        SendMessage("你没有权限...仅本群管理员可用", true);
                    }
                    else
                    {
                        string fullCommand = message.Substring(6, message.Length - 6);
                        HandleMessageCmd(fullCommand, PermissionLevel.Admin);
                    }
                }
                else
                {
                    string fullCommand = message.Substring(1, message.Length - 1);
                    HandleMessageCmd(fullCommand, PermissionLevel.Public);
                }

            }
            HandleMesasgeApp(message);

        }
        private void HandleMesasgeApp(string message)
        {
            foreach (var item in Mapper.NormalPlugins)
            {
                string reply = null;

                #region 折叠：invoke
                Type type = Type.GetType("DaylilyWeb.Function.Application." + item);
                MethodInfo mi = type.GetMethod("Execute");
                var ok = type.GetMethods();
                object appClass = Activator.CreateInstance(type);
                object[] invokeArgs = { message, userId, groupId, PermissionLevel.Public, false };

                #endregion

                bool enableAt = false;
                try
                {
                    reply = (string)mi.Invoke(appClass, invokeArgs);
                    enableAt = (bool)invokeArgs[4];
                }
                catch (TargetParameterCountException ex)
                {
                    throw new Exception("\"" + message + "\" caused an exception: \r\n" + type.Name.ToLower() + ": " + ex.Message);
                }
                if (reply == null) continue;
                SendMessage(reply, enableAt);
            }
        }
        private void HandleMessageCmd(string fullCommand, PermissionLevel currentLevel)
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
                catch (Exception)
                {
                    throw new Exception("\"" + fi.Name + "\" 这东西根本就是假的，是带特技的");
                }
            }

            object[] invokeArgs = { param, userId, groupId, currentLevel, false };
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
                    throw new Exception("\"" + fullCommand + "\" caused an exception: \r\n" + type.Name + ": " + ex.InnerException.Message);
                else
                    throw new Exception("\"" + fullCommand + "\" caused an exception: \r\n" + type.Name + ": " + ex.Message);
            }

            if (reply == null) return;
            SendMessage(reply, enableAt);
        }

        private void SendMessage(string message, bool enableAt)
        {
            if (messageType == MessageType.Group)
            {
                SendGroupMsgResponse msg = CQApi.SendGroupMessageAsync(groupId, (enableAt ? CQCode.EncodeAt(userId) + " " : "") + message).Result;
                Logger.InfoLine($"我: {message} {{status: {msg.Status}}})");
            }
            else if (messageType == MessageType.Discuss)
            {
                SendDiscussMsgResponse msg = CQApi.SendDiscussMessageAsync(discussId, (enableAt ? CQCode.EncodeAt(userId) + " " : "") + message).Result;
                Logger.InfoLine($"我: {message} {{status: {msg.Status}}})");
            }
            else if (messageType == MessageType.Private)
            {
                SendPrivateMsgResponse msg = CQApi.SendPrivateMessageAsync(userId, message).Result;
                Logger.InfoLine($"我: {message} {{status: {msg.Status}}})");
            }
            //var name = MethodBase.GetCurrentMethod().Name;
        }
    }
}
