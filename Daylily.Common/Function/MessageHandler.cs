using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Daylily.Common.Assist;
using Daylily.Common.Function.Command;
using Daylily.Common.Interface.CQHttp;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.CQResponse;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;
using Daylily.Common.Models.MessageList;
using Daylily.Common.Utils;
using Daylily.Common.Utils.LogUtils;

namespace Daylily.Common.Function
{
    public class MessageHandler
    {
        private static GroupList GroupInfo { get; } = new GroupList();
        private static DiscussList DiscussInfo { get; } = new DiscussList();
        private static PrivateList PrivateInfo { get; } = new PrivateList();

        public static string CommandFlag = "!";

        private readonly Random _rnd = new Random();
        private const int MinTime = 100; // 每条缓冲时间
        private const int MaxTime = 300; // 每条缓冲时间

        /// <summary>
        /// 群聊消息
        /// </summary>
        public MessageHandler(GroupMsg parsedObj)
        {
            long id = parsedObj.GroupId;

            GroupInfo.Add(id);
            if (GroupInfo[id].MsgQueue.Count < GroupInfo[id].MsgLimit) // 允许缓存n条，再多的丢弃
                GroupInfo[id].MsgQueue.Enqueue(parsedObj);

            else if (!GroupInfo[id].LockMsg)
            {
                GroupInfo[id].LockMsg = true;
                CqApi.SendMessage(new CommonMessageResponse(parsedObj.Message, new CommonMessage(parsedObj)));
            }

            if (!GroupInfo[id].TryRun(() => HandleGroupMessage(parsedObj)))
            {
                Logger.Info("当前已有" + GroupInfo[id].MsgQueue.Count + "条消息在" + GroupInfo[id].Info.GroupName + "排队");
            }
        }

        /// <summary>
        /// 讨论组消息
        /// </summary>
        public MessageHandler(DiscussMsg parsedObj)
        {
            long id = parsedObj.DiscussId;

            DiscussInfo.Add(id);
            if (DiscussInfo[id].MsgQueue.Count < DiscussInfo[id].MsgLimit) // 允许缓存n条，再多的丢弃
                DiscussInfo[id].MsgQueue.Enqueue(parsedObj);

            else if (!DiscussInfo[id].LockMsg)
            {
                DiscussInfo[id].LockMsg = true;
                CqApi.SendMessage(new CommonMessageResponse(parsedObj.Message, new CommonMessage(parsedObj)));
            }

            if (!DiscussInfo[id].TryRun(() => HandleDiscussMessage(parsedObj)))
            {
                Logger.Info("当前已有" + DiscussInfo[id].MsgQueue.Count + "条消息在" + id + "排队");
            }
        }

        /// <summary>
        /// 私聊消息
        /// </summary>
        public MessageHandler(PrivateMsg parsedObj)
        {
            long id = parsedObj.UserId;

            PrivateInfo.Add(id);
            if (PrivateInfo[id].MsgQueue.Count < PrivateInfo[id].MsgLimit) // 允许缓存n条，再多的丢弃
                PrivateInfo[id].MsgQueue.Enqueue(parsedObj);

            else if (!PrivateInfo[id].LockMsg)
            {
                PrivateInfo[id].LockMsg = true;
                CqApi.SendMessage(new CommonMessageResponse("？？求您慢点说话好吗", new CommonMessage(parsedObj)));
            }

            if (!PrivateInfo[id].TryRun(() => HandlePrivateMessage(parsedObj)))
            {
                Logger.Info("当前已有" + PrivateInfo[id].MsgQueue.Count + "条消息在" + id + "排队");
            }
        }

        private void HandleGroupMessage(GroupMsg parsedObj)
        {
            long groupId = parsedObj.GroupId;

            while (GroupInfo[groupId].MsgQueue.TryDequeue(out var currentInfo))
            {
                try
                {
                    CommonMessage commonMessage = new CommonMessage(currentInfo);
                    HandleMessage(commonMessage);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }

            GroupInfo[groupId].LockMsg = false;
        }

        private void HandleDiscussMessage(DiscussMsg parsedObj)
        {
            long discussId = parsedObj.DiscussId;
            while (DiscussInfo[discussId].MsgQueue.TryDequeue(out var currentInfo))
            {
                try
                {
                    CommonMessage commonMessage = new CommonMessage(currentInfo);
                    HandleMessage(commonMessage);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }

            DiscussInfo[discussId].LockMsg = false;
        }

        private void HandlePrivateMessage(PrivateMsg parsedObj)
        {
            long userId = parsedObj.UserId;
            while (PrivateInfo[userId].MsgQueue.TryDequeue(out var currentInfo))
            {
                try
                {
                    CommonMessage commonMessage = new CommonMessage(currentInfo);
                    HandleMessage(commonMessage);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }

            PrivateInfo[userId].LockMsg = false;
        }

        private void HandleMessage(CommonMessage commonMessage)
        {
            long groupId = Convert.ToInt64(commonMessage.GroupId);
            long userId = Convert.ToInt64(commonMessage.UserId);
            long discussId = Convert.ToInt64(commonMessage.DiscussId);
            var type = commonMessage.MessageType;
            string message = commonMessage.Message;

            switch (commonMessage.MessageType)
            {
                case MessageType.Private:
                    Logger.Message($"{userId}:\r\n  {CqCode.Decode(message)}");
                    break;
                case MessageType.Discuss:
                    Logger.Message($"({DiscussInfo[discussId].Name}) {userId}:\r\n  {CqCode.Decode(message)}");
                    break;
                case MessageType.Group:
                    var userInfo = CqApi.GetGroupMemberInfo(groupId.ToString(), userId.ToString()); // 有点费时间
                    Logger.Message(string.Format("({0}) {1}:\r\n  {2}", GroupInfo[groupId].Info.GroupName,
                        string.IsNullOrEmpty(userInfo.Data.Card) ? "(n)" + userInfo.Data.Nickname : userInfo.Data.Card,
                        CqCode.Decode(message)));
                    break;
            }

            if (commonMessage.Message.Substring(0, 1) == CommandFlag)
            {
                if (commonMessage.Message.IndexOf(CommandFlag + "root ", StringComparison.Ordinal) == 0)
                {
                    if (commonMessage.UserId != "2241521134")
                    {
                        CqApi.SendMessage(new CommonMessageResponse(LoliReply.FakeRoot, commonMessage));
                    }
                    else
                    {
                        commonMessage.FullCommand =
                            commonMessage.Message.Substring(6, commonMessage.Message.Length - 6);
                        commonMessage.PermissionLevel = PermissionLevel.Root;
                        HandleMessageCmd(commonMessage);
                    }

                }
                else if (message.IndexOf(CommandFlag + "sudo ", StringComparison.Ordinal) == 0 &&
                         type == MessageType.Group)
                {
                    if (GroupInfo[groupId].Info.Admins.Count(q => q.UserId == userId) == 0)
                    {
                        CqApi.SendMessage(new CommonMessageResponse(LoliReply.FakeAdmin, commonMessage));
                    }
                    else
                    {
                        commonMessage.FullCommand = message.Substring(6, message.Length - 6);
                        commonMessage.PermissionLevel = PermissionLevel.Admin;
                        HandleMessageCmd(commonMessage);
                    }
                }
                else
                {
                    commonMessage.FullCommand = message.Substring(1, message.Length - 1);
                    HandleMessageCmd(commonMessage);
                }
            }

            HandleMesasgeApp(commonMessage);
            Thread.Sleep(_rnd.Next(MinTime, MaxTime));
        }

        private static void HandleMesasgeApp(CommonMessage commonMessage)
        {
            foreach (var item in PluginManager.ApplicationList)
            {
                Task.Run(() =>
                {
                    CommonMessageResponse replyObj = item.Message_Received(commonMessage);
                    if (replyObj != null) CqApi.SendMessage(replyObj);
                });
            }
        }

        private static void HandleMessageCmd(CommonMessage commonMessage)
        {
            var cm = commonMessage;
            string fullCmd = cm.FullCommand;
            CommandAnalyzer ca = new CommandAnalyzer(new ParamDividerV2());
            ca.Analyze(fullCmd, cm);

            CommonMessageResponse replyObj = null;
            if (!PluginManager.CommandMap.ContainsKey(cm.Command)) return;
            Type t = PluginManager.CommandMap[cm.Command];
            CommandApp plugin = t == typeof(ExtendApp) ? PluginManager.CommandMapStatic[cm.Command] : GetInstance(t);

            Task.Run(() =>
            {
                try
                {
                    SetValues(cm, t, plugin);
                    replyObj = plugin.Message_Received(cm);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, fullCmd, plugin?.Name ?? "Unknown plugin");
                }

                if (replyObj == null) return;
                CqApi.SendMessage(replyObj);
            }
            );
        }

        private static void SetValues(CommonMessage cm, Type t, CommandApp plugin)
        {
            var props = t.GetProperties();
            int freeIndex = 0;
            string[] freeArray = cm.FreeArgs.ToArray();
            int length = freeArray.Length;
            foreach (var prop in props)
            {
                var info = prop.GetCustomAttributes(false);
                if (info.Length == 0) continue;
                switch (info[0])
                {
                    case ArgAttribute argAttrib:
                        if (cm.Switches.ContainsKey(argAttrib.Name))
                        {
                            if (argAttrib.IsSwitch)
                                prop.SetValue(plugin, true);
                        }
                        else if (cm.Args.ContainsKey(argAttrib.Name))
                        {
                            if (!argAttrib.IsSwitch)
                            {
                                dynamic obj = ParseStr(prop, cm.Args[argAttrib.Name]);
                                prop.SetValue(plugin, obj);
                            }
                        }
                        else if (argAttrib.Default != null)
                        {
                            prop.SetValue(plugin, argAttrib.Default); //不再转换，提升效率
                        }

                        break;
                    case FreeArgAttribute freeArgAttrib:
                        {
                            if (freeIndex > length - 1)
                            {
                                if (freeArgAttrib.Default != null)
                                    prop.SetValue(plugin, freeArgAttrib.Default); //不再转换，提升效率
                                break;
                            }

                            dynamic obj = ParseStr(prop, freeArray[freeIndex]);
                            prop.SetValue(plugin, obj);
                            freeIndex++;
                            break;
                        }
                }
            }
        }

        private static dynamic ParseStr(System.Reflection.PropertyInfo prop, string argStr)
        {
            dynamic obj;
            if (prop.PropertyType == typeof(int))
            {
                obj = Convert.ToInt32(argStr);
            }
            else if (prop.PropertyType == typeof(long))
            {
                obj = Convert.ToInt64(argStr);
            }
            else if (prop.PropertyType == typeof(short))
            {
                obj = Convert.ToInt16(argStr);
            }
            else if (prop.PropertyType == typeof(string))
            {
                obj = argStr;//Convert.ToString(cmd);
            }
            else if (prop.PropertyType == typeof(bool))
            {
                string tmpCmd = argStr == "" ? "true" : argStr;
                if (tmpCmd == "0")
                    tmpCmd = "false";
                else if (tmpCmd == "1")
                    tmpCmd = "true";
                obj = Convert.ToBoolean(tmpCmd);
            }
            else
            {
                throw new NotSupportedException("sb");
            }

            return obj;
        }

        private static CommandApp GetInstance(Type type) => Activator.CreateInstance(type) as CommandApp;
    }
}
