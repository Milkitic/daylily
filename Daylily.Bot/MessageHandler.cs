using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Daylily.Bot.Attributes;
using Daylily.Bot.Command;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.Models.MessageList;
using Daylily.Bot.PluginBase;
using Daylily.Common.IO;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ;
using Daylily.CoolQ.Interface.CqHttp;
using Daylily.CoolQ.Models.CqResponse;
using Daylily.CoolQ.Models.CqResponse.Api.Abstract;

namespace Daylily.Bot
{
    public class MessageHandler
    {
        public static GroupList GroupInfo { get; } = new GroupList();
        public static DiscussList DiscussInfo { get; } = new DiscussList();
        public static PrivateList PrivateInfo { get; } = new PrivateList();
        public static SessionList SessionInfo { get; } = new SessionList();


        public static ConcurrentDictionary<long, List<string>> GroupDisabledList { get; set; } =
            new ConcurrentDictionary<long, List<string>>();
        public static ConcurrentDictionary<long, List<string>> DiscussDisabledList { get; set; } =
            new ConcurrentDictionary<long, List<string>>();
        public static ConcurrentDictionary<long, List<string>> PrivateDisabledList { get; set; } =
            new ConcurrentDictionary<long, List<string>>();
        public static ConcurrentDictionary<string, int> CommandHot { get; set; }


        public static string CommandFlag = "!";

        private readonly Random _rnd = new Random();
        private const int MinTime = 100; // 每条缓冲时间
        private const int MaxTime = 300; // 每条缓冲时间

        static MessageHandler()
        {
            CommandHot = Settings.LoadSettings<ConcurrentDictionary<string, int>>("CommandHot") ??
                         new ConcurrentDictionary<string, int>();
        }

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
                SendMessage(new CommonMessageResponse(parsedObj.Message, new CommonMessage(parsedObj)));
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
                SendMessage(new CommonMessageResponse(parsedObj.Message, new CommonMessage(parsedObj)));
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
                SendMessage(new CommonMessageResponse("？？求您慢点说话好吗", new CommonMessage(parsedObj)));
            }

            if (!PrivateInfo[id].TryRun(() => HandlePrivateMessage(parsedObj)))
            {
                Logger.Info("当前已有" + PrivateInfo[id].MsgQueue.Count + "条消息在" + id + "排队");
            }
        }

        private void DispatchMessage(Msg msg)
        {
            void Dequeue<T>((long id, MessageType type) valueTuple) where T : Msg
            {
                while (SessionInfo[valueTuple].MsgQueue.TryDequeue(out object current))
                {
                    T currentMsg = (T)current;
                    try
                    {
                        CommonMessage commonMessage = new CommonMessage(currentMsg);
                        HandleMessage(commonMessage);
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex);
                    }
                }
            }

            (long id, MessageType type) identity;
            switch (msg)
            {
                case PrivateMsg privateMsg:
                    identity = (privateMsg.UserId, MessageType.Private);
                    Dequeue<PrivateMsg>(identity);
                    break;
                case DiscussMsg discussMsg:
                    identity = (discussMsg.DiscussId, MessageType.Discuss);
                    Dequeue<DiscussMsg>(identity);
                    break;
                case GroupMsg groupMsg:
                    identity = (groupMsg.GroupId, MessageType.Group);
                    Dequeue<GroupMsg>(identity);
                    break;
                default:
                    throw new ArgumentException();
            }

            SessionInfo[identity].LockMsg = false;
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
                    Logger.Message($"{userId}:\r\n  {CqCode.DecodeToString(message)}");
                    break;
                case MessageType.Discuss:
                    Logger.Message($"({DiscussInfo[discussId].Name}) {userId}:\r\n  {CqCode.DecodeToString(message)}");
                    break;
                case MessageType.Group:
                    var userInfo = CqApi.GetGroupMemberInfo(groupId.ToString(), userId.ToString()); // 有点费时间
                    Logger.Message(string.Format("({0}) {1}:\r\n  {2}", GroupInfo[groupId].Info.GroupName,
                        string.IsNullOrEmpty(userInfo.Data.Card) ? "(n)" + userInfo.Data.Nickname : userInfo.Data.Card,
                        CqCode.DecodeToString(message)));
                    break;
            }

            if (commonMessage.Message.Substring(0, 1) == CommandFlag)
            {
                if (commonMessage.Message.IndexOf(CommandFlag + "root ", StringComparison.Ordinal) == 0)
                {
                    if (commonMessage.UserId != "2241521134")
                    {
                        SendMessage(new CommonMessageResponse(LoliReply.FakeRoot, commonMessage));
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
                        SendMessage(new CommonMessageResponse(LoliReply.FakeAdmin, commonMessage));
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
            var cm = commonMessage;
            foreach (var item in PluginManager.ApplicationList)
            {
                Type t = item.GetType();
                if (ValidateDisabled(cm, t))
                    continue;

                Task.Run(() =>
                {
                    CommonMessageResponse replyObj = item.Message_Received(commonMessage);
                    if (replyObj != null) SendMessage(replyObj);
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
            if (ValidateDisabled(cm, t))
            {
                SendMessage(new CommonMessageResponse("本群已禁用此命令...", commonMessage));
                return;
            }

            CommandPlugin plugin = t == typeof(ExtendPlugin) ? PluginManager.CommandMapStatic[cm.Command] : GetInstance(t);
            if (!CommandHot.Keys.Contains(cm.Command))
            {
                CommandHot.TryAdd(cm.Command, 1);
            }
            else
            {
                CommandHot[cm.Command]++;
            }

            Settings.SaveSettings(CommandHot, "CommandHot");
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
                SendMessage(replyObj);
            }
            );
        }

        public static void SendMessage(CommonMessageResponse response)
        {
            switch (response.MessageType)
            {
                case MessageType.Group:
                    SendGroupMsgResp groupMsgResp = CqApi.SendGroupMessageAsync(response.GroupId,
                        (response.EnableAt ? new At(response.UserId) + " " : "") + response.Message);
                    Logger.Message(string.Format("({0}) 我: {{status: {1}}}\r\n  {2}",
                        DiscussInfo[long.Parse(response.GroupId)].Name, groupMsgResp.Status,
                        CqCode.DecodeToString(response.Message)));
                    break;
                case MessageType.Discuss:
                    SendDiscussMsgResp discussMsgResp = CqApi.SendDiscussMessageAsync(response.DiscussId,
                        (response.EnableAt ? new At(response.UserId) + " " : "") + response.Message);
                    Logger.Message(string.Format("({0}) 我: {{status: {1}}}\r\n  {2}",
                        DiscussInfo[long.Parse(response.DiscussId)].Name, discussMsgResp.Status,
                        CqCode.DecodeToString(response.Message)));
                    break;
                case MessageType.Private:
                    SendPrivateMsgResp privateMsgResp = CqApi.SendPrivateMessageAsync(response.UserId, response.Message);
                    Logger.Message(string.Format("({0}) 我: {{status: {1}}}\r\n  {2}", response.UserId,
                        privateMsgResp.Status, CqCode.DecodeToString(response.Message)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void SendMessage(CommonMessageResponse response, string groupId, string discussId,
            MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Group:
                    SendGroupMsgResp groupMsgResp = CqApi.SendGroupMessageAsync(groupId,
                        (response.EnableAt ? new At(response.UserId) + " " : "") + response.Message);
                    Logger.Info($"我: {CqCode.DecodeToString(response.Message)} {{status: {groupMsgResp.Status}}})");
                    break;
                case MessageType.Discuss:
                    SendDiscussMsgResp discussMsgResp = CqApi.SendDiscussMessageAsync(discussId,
                        (response.EnableAt ? new At(response.UserId) + " " : "") + response.Message);
                    Logger.Info($"我: {CqCode.DecodeToString(response.Message)} {{status: {discussMsgResp.Status}}})");
                    break;
                case MessageType.Private:
                    SendPrivateMsgResp privateMsgResp = CqApi.SendPrivateMessageAsync(response.UserId, response.Message);
                    Logger.Info($"我: {CqCode.DecodeToString(response.Message)} {{status: {privateMsgResp.Status}}})");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void SetValues(CommonMessage cm, Type t, CommandPlugin plugin)
        {
            var props = t.GetProperties();
            int freeIndex = 0;
            string[] freeArray = cm.FreeArgs.ToArray();
            int length = freeArray.Length;
            foreach (var prop in props)
            {
                var infos = prop.GetCustomAttributes(false);
                if (infos.Length == 0) continue;
                foreach (var info in infos)
                {
                    switch (info)
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
        }

        private static bool ValidateDisabled(CommonMessage cm, MemberInfo t)
        {
            switch (cm.MessageType)
            {
                case MessageType.Group:
                    if (!GroupDisabledList.Keys.Contains(long.Parse(cm.GroupId)))
                    {
                        GroupDisabledList.TryAdd(long.Parse(cm.GroupId), new List<string>());
                    }

                    if (GroupDisabledList[long.Parse(cm.GroupId)].Contains(t.Name))
                        return true;
                    break;
                case MessageType.Private:
                    if (!PrivateDisabledList.Keys.Contains(long.Parse(cm.UserId)))
                    {
                        PrivateDisabledList.TryAdd(long.Parse(cm.UserId), new List<string>());
                    }

                    if (PrivateDisabledList[long.Parse(cm.UserId)].Contains(t.Name))
                        return true;
                    break;
                case MessageType.Discuss:
                    if (!DiscussDisabledList.Keys.Contains(long.Parse(cm.DiscussId)))
                    {
                        DiscussDisabledList.TryAdd(long.Parse(cm.DiscussId), new List<string>());
                    }

                    if (DiscussDisabledList[long.Parse(cm.DiscussId)].Contains(t.Name))
                        return true;
                    break;
            }

            return false;
        }

        private static dynamic ParseStr(PropertyInfo prop, string argStr)
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
            else if (prop.PropertyType == typeof(float))
            {
                obj = Convert.ToSingle(argStr);
            }
            else if (prop.PropertyType == typeof(double))
            {
                obj = Convert.ToDouble(argStr);
            }
            else if (prop.PropertyType == typeof(string))
            {
                obj = CqCode.Decode(argStr); // Convert.ToString(cmd);
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

        private static CommandPlugin GetInstance(Type type) => Activator.CreateInstance(type) as CommandPlugin;
    }
}
