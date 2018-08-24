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
using Daylily.Bot.Interface;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Common.IO;
using Daylily.CoolQ;
using Daylily.CoolQ.Interface.CqHttp;
using Daylily.CoolQ.Models.CqResponse;
using Daylily.CoolQ.Models.CqResponse.Api.Abstract;
using static Daylily.Common.Utils.LoggerUtils.Logger;

namespace Daylily.Bot
{
    public class CoolQDispatcher : IDispatcher
    {
        public static SessionList SessionInfo { get; } = new SessionList();

        public static ConcurrentDictionary<long, List<string>> GroupDisabledList { get; set; } =
            new ConcurrentDictionary<long, List<string>>();
        public static ConcurrentDictionary<long, List<string>> DiscussDisabledList { get; set; } =
            new ConcurrentDictionary<long, List<string>>();
        public static ConcurrentDictionary<long, List<string>> PrivateDisabledList { get; set; } =
            new ConcurrentDictionary<long, List<string>>();
        public static ConcurrentDictionary<string, int> CommandHot { get; }

        public static string CommandFlag = "!";

        private static readonly Random Rnd = new Random();
        private const int MinTime = 100; // 每条缓冲时间
        private const int MaxTime = 300; // 每条缓冲时间

        static CoolQDispatcher()
        {
            CommandHot = Settings.LoadSettings<ConcurrentDictionary<string, int>>("CommandHot") ??
                         new ConcurrentDictionary<string, int>();
        }

        public CoolQDispatcher()
        {
            Core.MessageReceived += Message_Received;
        }

        public void Message_Received(object sender, MessageReceivedEventArgs args)
        {
            var originObj = args.MessageObj;
            Identity id;
            switch (originObj)
            {
                case PrivateMsg privateMsg:
                    id = new Identity(privateMsg.UserId, MessageType.Private);
                    break;
                case DiscussMsg discussMsg:
                    id = new Identity(discussMsg.DiscussId, MessageType.Discuss);
                    break;
                case GroupMsg groupMsg:
                    id = new Identity(groupMsg.GroupId, MessageType.Group);
                    break;
                default:
                    throw new ArgumentException();
            }

            SessionInfo.Add(originObj);
            if (SessionInfo[id].MsgQueue.Count < SessionInfo[id].MsgLimit) // 允许缓存n条，再多的丢弃
            {
                SessionInfo[id].MsgQueue.Enqueue(originObj);
            }
            else if (!SessionInfo[id].LockMsg)
            {
                SessionInfo[id].LockMsg = true;
                SendMessage(new CommonMessageResponse(originObj.Message, new CommonMessage(originObj)));
            }

            if (!SessionInfo[id].TryRun(() => DispatchMessage(originObj)))
            {
                Info("当前已有" + SessionInfo[id].MsgQueue.Count + "条消息在" + SessionInfo[id].Name + "排队");
            }
        }

        private static void DispatchMessage(Msg msg)
        {
            Identity identity;
            switch (msg)
            {
                case PrivateMsg privateMsg:
                    identity = new Identity(privateMsg.UserId, MessageType.Private);
                    RunNext<PrivateMsg>(identity);
                    break;
                case DiscussMsg discussMsg:
                    identity = new Identity(discussMsg.DiscussId, MessageType.Discuss);
                    RunNext<DiscussMsg>(identity);
                    break;
                case GroupMsg groupMsg:
                    identity = new Identity(groupMsg.GroupId, MessageType.Group);
                    RunNext<GroupMsg>(identity);
                    break;
                default:
                    throw new ArgumentException();
            }

            SessionInfo[identity].LockMsg = false;

            void RunNext<T>(Identity id) where T : Msg
            {
                while (SessionInfo[id].MsgQueue.TryDequeue(out object current))
                {
                    var currentMsg = (T)current;
                    try
                    {
                        CommonMessage commonMessage = new CommonMessage(currentMsg);
                        HandleMessage(commonMessage);
                    }
                    catch (Exception ex)
                    {
                        Exception(ex);
                    }
                }
            }
        }

        private static void HandleMessage(CommonMessage cm)
        {
            long groupId = Convert.ToInt64(cm.GroupId);
            long userId = Convert.ToInt64(cm.UserId);
            long discussId = Convert.ToInt64(cm.DiscussId);
            var type = cm.MessageType;

            string group, sender, message = cm.Message;
            switch (cm.MessageType)
            {
                case MessageType.Private:
                    group = "私聊";
                    sender = SessionInfo[cm.Identity].Name;
                    break;
                case MessageType.Discuss:
                    group = SessionInfo[cm.Identity].Name;
                    sender = cm.UserId;
                    break;
                default:
                case MessageType.Group:
                    var userInfo = CqApi.GetGroupMemberInfo(cm.GroupId, cm.UserId); // 有点费时间
                    group = SessionInfo[cm.Identity].Name;
                    sender = string.IsNullOrEmpty(userInfo.Data.Card)
                        ? userInfo.Data.Nickname
                        : userInfo.Data.Card;
                    break;
            }

            Message($"({group}) {sender}:\r\n  {CqCode.DecodeToString(message)}");

            if (cm.Message.Substring(0, 1) == CommandFlag)
            {
                if (cm.Message.IndexOf(CommandFlag + "root ", StringComparison.InvariantCulture) == 0)
                {
                    if (cm.UserId != "2241521134")
                    {
                        SendMessage(new CommonMessageResponse(LoliReply.FakeRoot, cm));
                    }
                    else
                    {
                        cm.FullCommand = cm.Message.Substring(6, cm.Message.Length - 6);
                        cm.PermissionLevel = PermissionLevel.Root;
                        HandleMessageCmd(cm);
                    }

                }
                else if (message.IndexOf(CommandFlag + "sudo ", StringComparison.InvariantCulture) == 0 &&
                         type == MessageType.Group)
                {
                    if (SessionInfo[cm.Identity].GroupInfo.Admins.Count(q => q.UserId == userId) == 0)
                    {
                        SendMessage(new CommonMessageResponse(LoliReply.FakeAdmin, cm));
                    }
                    else
                    {
                        cm.FullCommand = message.Substring(6, message.Length - 6);
                        cm.PermissionLevel = PermissionLevel.Admin;
                        HandleMessageCmd(cm);
                    }
                }
                else
                {
                    cm.FullCommand = message.Substring(1, message.Length - 1);
                    HandleMessageCmd(cm);
                }
            }

            HandleMesasgeApp(cm);
            Thread.Sleep(Rnd.Next(MinTime, MaxTime));
        }

        private static void HandleMesasgeApp(CommonMessage cm)
        {
            foreach (var item in PluginManager.ApplicationList)
            {
                Type t = item.GetType();
                if (ValidateDisabled(cm, t))
                    continue;

                Task.Run(() =>
                {
                    CommonMessageResponse replyObj = item.Message_Received(cm);
                    if (replyObj != null) SendMessage(replyObj);
                });
            }
        }

        private static void HandleMessageCmd(CommonMessage cm)
        {
            string fullCmd = cm.FullCommand;
            CommandAnalyzer ca = new CommandAnalyzer(new ParamDividerV2());
            ca.Analyze(fullCmd, cm);
            CommonMessageResponse replyObj = null;
            if (!PluginManager.CommandMap.ContainsKey(cm.Command)) return;

            Type t = PluginManager.CommandMap[cm.Command];
            if (ValidateDisabled(cm, t))
            {
                SendMessage(new CommonMessageResponse("本群已禁用此命令...", cm));
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
                    Exception(ex, fullCmd, plugin?.Name ?? "Unknown plugin");
                }

                if (replyObj == null) return;
                SendMessage(replyObj);
            }
            );
        }

        public static void SendMessage(CommonMessageResponse resp)
        {
            var msg = (resp.EnableAt && resp.MessageType != MessageType.Private ? new At(resp.UserId) + " " : "") +
                    resp.Message;
            var info = SessionInfo[resp.Identity].Name;
            string status;
            switch (resp.MessageType)
            {
                case MessageType.Group:
                    status = CqApi.SendGroupMessageAsync(resp.GroupId, msg).Status;
                    break;
                case MessageType.Discuss:
                    status = CqApi.SendDiscussMessageAsync(resp.DiscussId, msg).Status;
                    break;
                case MessageType.Private:
                    status = CqApi.SendPrivateMessageAsync(resp.UserId, msg).Status;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Message(string.Format("({0}) 我: {{status: {1}}}\r\n  {2}", info, status, CqCode.DecodeToString(msg)));
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
