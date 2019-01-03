﻿using Daylily.Bot.Attributes;
using Daylily.Bot.Command;
using Daylily.Bot.Enum;
using Daylily.Bot.Interface;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Common.IO;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ;
using Daylily.CoolQ.Interface.CqHttp;
using Daylily.CoolQ.Models.CqResponse;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Daylily.Bot
{
    public class CoolQDispatcher : IDispatcher
    {
        public static CoolQDispatcher Current { get; private set; }
        public MiddlewareConfig MiddlewareConfig { get; set; } = new MiddlewareConfig();

        public event SessionReceivedEventHandler SessionReceived;

        public SessionList SessionInfo { get; } = new SessionList();
        public List<GroupMemberGroupInfo> GroupMemberGroupInfo { get; set; } = new List<GroupMemberGroupInfo>();
        public ConcurrentDictionary<long, List<string>> GroupDisabledList { get; set; } =
            new ConcurrentDictionary<long, List<string>>();
        public ConcurrentDictionary<long, List<string>> DiscussDisabledList { get; set; } =
            new ConcurrentDictionary<long, List<string>>();
        public ConcurrentDictionary<long, List<string>> PrivateDisabledList { get; set; } =
            new ConcurrentDictionary<long, List<string>>();
        public ConcurrentDictionary<string, int> CommandHot { get; }


        private readonly Random _rnd = new Random();
        private const int MinTime = 100; // 每条缓冲时间
        private const int MaxTime = 300; // 每条缓冲时间

        public CoolQDispatcher()
        {
            Current = this;
            CommandHot = Settings.LoadSettings<ConcurrentDictionary<string, int>>("CommandHot") ??
                         new ConcurrentDictionary<string, int>();
        }

        public bool Message_Received(object sender, MessageReceivedEventArgs args)
        {
            bool handled = false;
            var originObj = args.MessageObj;
            CqIdentity id;
            switch (originObj)
            {
                case PrivateMsg privateMsg:
                    id = new CqIdentity(privateMsg.UserId, MessageType.Private);
                    break;
                case DiscussMsg discussMsg:
                    id = new CqIdentity(discussMsg.DiscussId, MessageType.Discuss);
                    break;
                case GroupMsg groupMsg:
                    id = new CqIdentity(groupMsg.GroupId, MessageType.Group);
                    break;
                default:
                    throw new ArgumentException();
            }

            SessionInfo.TryAdd(originObj);
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
                Logger.Info("当前已有" + SessionInfo[id].MsgQueue.Count + "条消息在" + SessionInfo[id].Name + "排队");
            }

            return handled;
        }

        private void DispatchMessage(Msg msg)
        {
            CqIdentity cqIdentity;
            switch (msg)
            {
                case PrivateMsg privateMsg:
                    cqIdentity = new CqIdentity(privateMsg.UserId, MessageType.Private);
                    RunNext<PrivateMsg>(cqIdentity);
                    break;
                case DiscussMsg discussMsg:
                    cqIdentity = new CqIdentity(discussMsg.DiscussId, MessageType.Discuss);
                    RunNext<DiscussMsg>(cqIdentity);
                    break;
                case GroupMsg groupMsg:
                    cqIdentity = new CqIdentity(groupMsg.GroupId, MessageType.Group);
                    RunNext<GroupMsg>(cqIdentity);
                    break;
                default:
                    throw new ArgumentException();
            }

            SessionInfo[cqIdentity].LockMsg = false;

            void RunNext<T>(CqIdentity id) where T : Msg
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
                        Logger.Exception(ex);
                    }
                }
            }
        }

        private async void HandleMessage(CommonMessage cm)
        {
            //bool cmdFlag = false;
            //long groupId = Convert.ToInt64(cm.GroupId);
            //long userId = Convert.ToInt64(cm.UserId);
            //long discussId = Convert.ToInt64(cm.DiscussId);
            //var type = cm.MessageType;

            //string group, sender, message = cm.RawMessage;
            //if (cm.MessageType == MessageType.Private)
            //{
            //    group = "私聊";
            //    sender = SessionInfo[cm.CqIdentity].Name;
            //}
            //else if (cm.MessageType == MessageType.Discuss)
            //{
            //    group = SessionInfo[cm.CqIdentity].Name;
            //    sender = cm.UserId;
            //}
            //else
            //{
            //    var userInfo = SessionInfo[cm.CqIdentity]?.GroupInfo?.Members?.FirstOrDefault(i => i.UserId == userId) ??
            //                   CqApi.GetGroupMemberInfo(cm.GroupId, cm.UserId).Data;
            //    group = SessionInfo?[cm.CqIdentity]?.Name;
            //    sender = string.IsNullOrEmpty(userInfo.Card)
            //        ? userInfo.Nickname
            //        : userInfo.Card;
            //}

            //Logger.Message($"({group}) {sender}:\r\n  {CqCode.DecodeToString(message)}");

            var handled = await HandleMessageApp(cm);
            if (!handled)
            {
                HandleMessageCmd(cm);
                //if (cm.RawMessage.Substring(0, 1) == CommandFlag)
                //{
                //    if (cm.RawMessage.IndexOf(CommandFlag + "root ", StringComparison.InvariantCulture) == 0)
                //    {
                //        if (cm.UserId != "2241521134")
                //        {
                //            SendMessage(new CommonMessageResponse(LoliReply.FakeRoot, cm));
                //        }
                //        else
                //        {
                //            cm.FullCommand = cm.RawMessage.Substring(6, cm.RawMessage.Length - 6);
                //            cm.PermissionLevel = PermissionLevel.Root;
                //            cmdFlag = true;
                //            HandleMessageCmd(cm);
                //        }

                //    }
                //    else if (message.IndexOf(CommandFlag + "sudo ", StringComparison.InvariantCulture) == 0 &&
                //             type == MessageType.Group)
                //    {
                //        if (SessionInfo[cm.CqIdentity].GroupInfo.Admins.Count(q => q.UserId == userId) == 0)
                //        {
                //            SendMessage(new CommonMessageResponse(LoliReply.FakeAdmin, cm));
                //        }
                //        else
                //        {
                //            cm.FullCommand = message.Substring(6, message.Length - 6);
                //            cm.PermissionLevel = PermissionLevel.Admin;
                //            cmdFlag = true;
                //            HandleMessageCmd(cm);
                //        }
                //    }
                //    else
                //    {
                //        // auto
                //        if (SessionInfo[cm.CqIdentity].GroupInfo?.Admins.Count(q => q.UserId == userId) != 0)
                //            cm.PermissionLevel = PermissionLevel.Admin;
                //        if (cm.UserId == "2241521134")
                //            cm.PermissionLevel = PermissionLevel.Root;

                //        cm.FullCommand = message.Substring(1, message.Length - 1);
                //        cmdFlag = true;
                //        HandleMessageCmd(cm);
                //    }
                //}
                //if (!cmdFlag)
                //    SessionReceived?.Invoke(null, new SessionReceivedEventArgs
                //    {
                //        MessageObj = cm
                //    });
            }
            Thread.Sleep(_rnd.Next(MinTime, MaxTime));
        }

        private async Task<bool> HandleMessageApp(CommonMessage cm)
        {
            int? priority = int.MinValue;
            bool handled = false;

            foreach (var appPlugin in PluginManager.ApplicationList.OrderByDescending(k => k.BackendConfig?.Priority))
            {
                int? p = appPlugin.BackendConfig?.Priority;
                if (p < priority && handled)
                {
                    break;
                }

                Logger.Raw(appPlugin.Name);
                priority = appPlugin.BackendConfig?.Priority;
                Type t = appPlugin.GetType();
                if (ValidateDisabled(cm, t))
                    continue;

                CommonMessageResponse replyObj = null;
                var task = Task.Run(() =>
                {
                    replyObj = appPlugin.OnMessageReceived(cm);
                    if (replyObj != null && !replyObj.Cancel) SendMessage(replyObj);
                });

                if (!appPlugin.RunInMultiThreading)
                {
                    await task.ConfigureAwait(false);
                    handled = replyObj?.Handled ?? false;
                }
            }

            return handled;
        }

        private void HandleMessageCmd(CommonMessage cm)
        {
            string fullCmd = cm.FullCommand;
            CommandAnalyzer ca = new CommandAnalyzer(new ParamDividerV2());
            ca.Analyze(fullCmd, cm);
            CommonMessageResponse replyObj = null;

            SessionReceived?.Invoke(null, new SessionReceivedEventArgs
            {
                MessageObj = cm
            });

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
                    if (!TrySetValues(cm, t, plugin))
                        return;
                    replyObj = plugin.OnMessageReceived(cm);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex.InnerException ?? ex, fullCmd, plugin?.Name ?? "Unknown plugin");
                }

                if (replyObj == null) return;
                SendMessage(replyObj);
            }
            );
        }

        public void SendMessage(CommonMessageResponse resp)
        {
            var msg = (resp.EnableAt && resp.MessageType != MessageType.Private ? new At(resp.UserId) + " " : "") +
                    resp.Message;
            var info = SessionInfo[resp.CqIdentity] == null
                ? $"{resp.CqIdentity.Type}{resp.CqIdentity.Id}"
                : SessionInfo[resp.CqIdentity].Name;
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

            Logger.Message(string.Format("({0}) 我: {{status: {1}}}\r\n  {2}", info, status, CqCode.DecodeToString(msg)));
        }

        private bool ValidateDisabled(CommonMessage cm, MemberInfo t)
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

        private bool TrySetValues(CommonMessage cm, Type t, CommandPlugin plugin)
        {
            var props = t.GetProperties();
            int freeIndex = 0;
            string[] freeArray = cm.FreeArgs.ToArray();
            int freeCount = freeArray.Length;
            int swCount = cm.Switches.Count;
            int argCount = cm.Args.Count;

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
                                {
                                    prop.SetValue(plugin, true);
                                    swCount--;
                                }
                            }
                            else if (cm.Args.ContainsKey(argAttrib.Name))
                            {
                                if (!argAttrib.IsSwitch)
                                {
                                    if (TryParse(prop, cm.Args[argAttrib.Name], out var parsed))
                                    {
                                        //dynamic obj = TryParse(prop, cm.Args[argAttrib.Name]);
                                        prop.SetValue(plugin, parsed);
                                        argCount--;
                                    }
                                    else
                                    {
                                        SendMessage(
                                            new CommonMessageResponse($"参数有误...发送 \"/help {cm.Command}\" 了解如何使用。", cm));
                                        return false;
                                    }
                                }
                            }
                            else if (argAttrib.Default != null)
                            {
                                prop.SetValue(plugin, argAttrib.Default); //不再转换，提升效率
                            }

                            break;
                        case FreeArgAttribute freeArgAttrib:
                            {
                                if (freeIndex > freeCount - 1)
                                {
                                    if (freeArgAttrib.Default != null)
                                        prop.SetValue(plugin, freeArgAttrib.Default); //不再转换，提升效率
                                    break;
                                }

                                if (TryParse(prop, freeArray[freeIndex], out var parsed))
                                {
                                    prop.SetValue(plugin, parsed);
                                    freeIndex++;
                                    break;
                                }
                                else
                                {
                                    SendMessage(new CommonMessageResponse($"参数有误...发送 \"/help {cm.Command}\" 了解如何使用。",
                                        cm));
                                    return false;
                                }
                            }
                    }
                }

            }

            if (swCount <= 0 && argCount <= 0)
                return true;
            SendMessage(new CommonMessageResponse($"包含多余的参数. 发送 \"/help {cm.Command}\" 了解如何使用。", cm));
            return false;
        }

        private static bool TryParse(PropertyInfo prop, string argStr, out dynamic parsed)
        {
            try
            {
                if (prop.PropertyType == typeof(int))
                {
                    parsed = Convert.ToInt32(argStr);
                }
                else if (prop.PropertyType == typeof(long))
                {
                    parsed = Convert.ToInt64(argStr);
                }
                else if (prop.PropertyType == typeof(short))
                {
                    parsed = Convert.ToInt16(argStr);
                }
                else if (prop.PropertyType == typeof(float))
                {
                    parsed = Convert.ToSingle(argStr);
                }
                else if (prop.PropertyType == typeof(double))
                {
                    parsed = Convert.ToDouble(argStr);
                }
                else if (prop.PropertyType == typeof(string))
                {
                    parsed = CqCode.Decode(argStr); // Convert.ToString(cmd);
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    string tmpCmd = argStr == "" ? "true" : argStr;
                    if (tmpCmd == "0")
                        tmpCmd = "false";
                    else if (tmpCmd == "1")
                        tmpCmd = "true";
                    parsed = Convert.ToBoolean(tmpCmd);
                }
                else
                {
                    throw new NotSupportedException("sb");
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                parsed = null;
                return false;
            }
        }

        private static CommandPlugin GetInstance(Type type) => Activator.CreateInstance(type) as CommandPlugin;
    }
}
