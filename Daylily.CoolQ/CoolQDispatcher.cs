using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Dispatcher;
using Daylily.Bot.Interface;
using Daylily.Bot.Message;
using Daylily.Bot.Models;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ.Interface.CqHttp;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Models.CqResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Daylily.CoolQ.Plugins;

namespace Daylily.CoolQ
{
    public class CoolQDispatcher : IDispatcher
    {
        public static CoolQDispatcher Current { get; private set; }
        public MiddlewareConfig MiddlewareConfig { get; set; } = new MiddlewareConfig();

        public event SessionReceivedEventHandler SessionReceived;

        public SessionList SessionInfo { get; } = new SessionList();
        public List<GroupMemberGroupInfo> GroupMemberGroupInfo { get; set; } = new List<GroupMemberGroupInfo>();

        private readonly Random _rnd = new Random();
        private const int MinTime = 100; // 每条缓冲时间
        private const int MaxTime = 300; // 每条缓冲时间

        public CoolQDispatcher()
        {
            Current = this;
        }

        public bool Message_Received(object sender, MessageReceivedEventArgs args)
        {
            bool handled = false;
            var originObj = (Msg)args.MessageObj;
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
                SendMessage(new CommonMessageResponse(originObj.Message, CoolQNavigableMessage.Parse(originObj)));
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
                        CoolQNavigableMessage coolQNavigableMessage = CoolQNavigableMessage.Parse(currentMsg);
                        HandleMessage(coolQNavigableMessage);
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex);
                    }
                }
            }
        }

        private async void HandleMessage(CoolQNavigableMessage cm)
        {
            var handled = await HandleMessageApp(cm);
            if (!handled)
            {
                if (!string.IsNullOrEmpty(cm.FullCommand))
                {
                    HandleMessageCmd(cm);
                }

                //if (!cmdFlag)
                //    SessionReceived?.Invoke(null, new SessionReceivedEventArgs
                //    {
                //        MessageObj = cm
                //    });
            }
            Thread.Sleep(_rnd.Next(MinTime, MaxTime));
        }

        private async Task<bool> HandleMessageApp(CoolQNavigableMessage cm)
        {
            int? priority = int.MinValue;
            bool handled = false;

            //var app = PluginManager.GetPlugin < PluginSwitch >
            foreach (var appPlugin in CoolQPluginManager.ApplicationList.OrderByDescending(k => k.BackendConfig?.Priority))
            {
                int? p = appPlugin.BackendConfig?.Priority;
                if (p < priority && handled)
                {
                    break;
                }

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

        private void HandleMessageCmd(CoolQNavigableMessage cm)
        {
            CommonMessageResponse replyObj = null;

            SessionReceived?.Invoke(null, new SessionReceivedEventArgs
            {
                NavigableMessageObj = cm
            });

            if (!CoolQPluginManager.CommandMap.ContainsKey(cm.Command)) return;

            Type t = CoolQPluginManager.CommandMap[cm.Command];
            if (ValidateDisabled(cm, t))
            {
                SendMessage(new CommonMessageResponse("本群已禁用此命令...", cm));
                return;
            }

            CoolQCommandPlugin plugin =
                t == typeof(ExtendPlugin)
                ? CoolQPluginManager.CommandMapStatic[cm.Command]
                : GetInstance(t);
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
                        Logger.Exception(ex.InnerException ?? ex, cm.FullCommand, plugin?.Name ?? "Unknown plugin");
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

            Logger.Message(string.Format("({0}) 我: {{status: {1}}}\r\n  {2}", info, status, CoolQCode.DecodeToString(msg)));
        }


        private static CoolQCommandPlugin GetInstance(Type type) => Activator.CreateInstance(type) as CoolQCommandPlugin;
    }
}
