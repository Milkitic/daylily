﻿using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Dispatcher;
using Daylily.Bot.Message;
using Daylily.Bot.Session;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ.CoolQHttp;
using Daylily.CoolQ.CoolQHttp.ResponseModel.Report;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Daylily.CoolQ
{
    public class CoolQDispatcher : CompoundDispatcher
    {
        public static CoolQDispatcher Current { get; private set; }
        public override MiddlewareConfig MiddlewareConfig { get; } = new MiddlewareConfig();

        public SessionList SessionInfo { get; } = new SessionList();
        public List<GroupMemberGroupInfo> GroupMemberGroupInfo { get; set; } = new List<GroupMemberGroupInfo>();

        private readonly Random _rnd = new Random();
        private const int MinTime = 100; // 每条缓冲时间
        private const int MaxTime = 300; // 每条缓冲时间
        public PluginManager PluginManager => DaylilyCore.Current.PluginManager;

        public CoolQDispatcher()
        {
            Current = this;
        }

        public override bool Message_Received(object sender, MessageEventArgs args)
        {
            bool handled = false;
            var originObj = (CoolQMessageApi)args.ParsedObject;
            CoolQIdentity id;
            switch (originObj)
            {
                case CoolQPrivateMessageApi privateMsg:
                    id = new CoolQIdentity(privateMsg.UserId, MessageType.Private);
                    break;
                case CoolQDiscussMessageApi discussMsg:
                    id = new CoolQIdentity(discussMsg.DiscussId, MessageType.Discuss);
                    break;
                case CoolQGroupMessageApi groupMsg:
                    id = new CoolQIdentity(groupMsg.GroupId, MessageType.Group);
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
                SendMessage(CoolQRouteMessage.Parse(originObj).ToSource(originObj.Message));
            }

            if (!SessionInfo[id].TryRun(() => DispatchMessage(originObj)))
            {
                Logger.Info("当前已有" + SessionInfo[id].MsgQueue.Count + "条消息在" + SessionInfo[id].Name + "排队");
            }

            return handled;
        }

        private void DispatchMessage(CoolQMessageApi coolQMessageApi)
        {
            CoolQIdentity cqIdentity;
            switch (coolQMessageApi)
            {
                case CoolQPrivateMessageApi privateMsg:
                    cqIdentity = new CoolQIdentity(privateMsg.UserId, MessageType.Private);
                    RunNext<CoolQPrivateMessageApi>(cqIdentity);
                    break;
                case CoolQDiscussMessageApi discussMsg:
                    cqIdentity = new CoolQIdentity(discussMsg.DiscussId, MessageType.Discuss);
                    RunNext<CoolQDiscussMessageApi>(cqIdentity);
                    break;
                case CoolQGroupMessageApi groupMsg:
                    cqIdentity = new CoolQIdentity(groupMsg.GroupId, MessageType.Group);
                    RunNext<CoolQGroupMessageApi>(cqIdentity);
                    break;
                default:
                    throw new ArgumentException();
            }

            SessionInfo[cqIdentity].LockMsg = false;

            void RunNext<T>(CoolQIdentity id) where T : CoolQMessageApi
            {
                while (SessionInfo[id].MsgQueue.TryDequeue(out object current))
                {
                    var currentMsg = (T)current;
                    try
                    {
                        CoolQRouteMessage coolQRouteMessage = CoolQRouteMessage.Parse(currentMsg);
                        HandleMessage(coolQRouteMessage);
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex);
                    }
                }
            }
        }

        private async void HandleMessage(CoolQRouteMessage cm)
        {
            RaiseSessionEvent(cm);
            var handled = await HandleApplication(cm);
            if (!handled)
            {
                if (!string.IsNullOrEmpty(cm.FullCommand))
                {
                    HandleCommand(cm);
                }

                //if (!cmdFlag)
                //    SessionReceived?.Invoke(null, new SessionReceivedEventArgs
                //    {
                //        MessageObj = cm
                //    });
            }
            Thread.Sleep(_rnd.Next(MinTime, MaxTime));
        }

        private async Task<bool> HandleApplication(CoolQRouteMessage cm)
        {
            int? priority = int.MinValue;
            bool handled = false;

            //var app = PluginManager.GetPlugin < PluginSwitch >
            foreach (var appPlugin in PluginManager.ApplicationInstances.OrderByDescending(k => k.MiddlewareConfig?.Priority))
            {
                int? p = appPlugin.MiddlewareConfig?.Priority;
                if (p < priority && handled)
                {
                    break;
                }

                priority = appPlugin.MiddlewareConfig?.Priority;
                Type t = appPlugin.GetType();
                //if (ValidateDisabled(cm, t))
                //    continue;

                CoolQRouteMessage replyObj = null;
                var task = Task.Run(() =>
                {
                    replyObj = ((CoolQApplicationPlugin)appPlugin).OnMessageReceived(cm);
                    if (replyObj != null) SendMessage(replyObj);
                });

                if (!appPlugin.RunInMultiThreading)
                {
                    await task.ConfigureAwait(false);
                    handled = replyObj?.Handled ?? false;
                }
            }

            return handled;
        }

        private void HandleCommand(CoolQRouteMessage cm)
        {
            CoolQRouteMessage replyObj = null;

            
            if (!PluginManager.ContainsPlugin(cm.Command)) return;

            Type t = PluginManager.GetPluginType(cm.Command);
            //if (ValidateDisabled(cm, t))
            //{
            //    SendMessage(routeMsg.ToSource("本群已禁用此命令...", cm));
            //    return;
            //}

            //CoolQCommandPlugin plugin =
            //    t == typeof(ExtendPlugin)
            //    ? CoolQPluginManager.CommandMapStatic[cm.Command]
            //    : GetInstance(t);
            CoolQCommandPlugin plugin = PluginManager.GetNewInstance<CoolQCommandPlugin>(t);
            if (plugin != null)
            {
                Task.Run(() =>
                    {
                        try
                        {
                            if (!plugin.TryInjectParameters(cm))
                                return;
                            replyObj = plugin.OnMessageReceived(cm);
                        }
                        catch (Exception ex)
                        {
                            Logger.Exception(ex.InnerException ?? ex, cm.FullCommand, plugin.Name);
                        }

                        if (replyObj == null) return;
                        SendMessage(replyObj);
                    }
                );
            }
            else
            {
                Logger.Error($"Cannot find plugin: {t.FullName}.");
            }

        }

        public override void SendMessage(RouteMessage message)
        {
            var routeMessage = (CoolQRouteMessage)message;
            var msg = (routeMessage.EnableAt && routeMessage.MessageType != MessageType.Private
                          ? new At(routeMessage.UserId) + " "
                          : "") + routeMessage.Message;
            var info = SessionInfo[(CoolQIdentity)routeMessage.Identity] == null
                ? $"{((CoolQIdentity)routeMessage.Identity).Type}{((CoolQIdentity)routeMessage.Identity).Id}"
                : SessionInfo[(CoolQIdentity)routeMessage.Identity].Name;
            string status;
            switch (routeMessage.MessageType)
            {
                case MessageType.Group:
                    status = CoolQHttpApi.SendGroupMessageAsync(routeMessage.GroupId, msg).Status;
                    break;
                case MessageType.Discuss:
                    status = CoolQHttpApi.SendDiscussMessageAsync(routeMessage.DiscussId, msg).Status;
                    break;
                case MessageType.Private:
                    status = CoolQHttpApi.SendPrivateMessageAsync(routeMessage.UserId, msg).Status;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Logger.Message(string.Format("({0}) 我: {{status: {1}}}\r\n  {2}", info, status, CoolQCode.DecodeToString(msg)));
        }

        public override bool Event_Received(object sender, EventEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}