using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.CoolQ;
using Daylily.CoolQ.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Daylily.Bot.Messaging;
using Daylily.CoolQ.Plugin;

namespace Daylily.Plugin.Basic
{
    [Name("日程提醒")]
    [Author("yf_extension")]
    [Version(2, 0, 1, PluginVersion.Alpha)]
    [Help("日程提醒管理。", Authority = Authority.Root)]
    [Command("rcon")]
    public class Rcon : CoolQCommandPlugin
    {
        public override Guid Guid => new Guid("d765e508-37bc-46f9-be94-e96819c250b6");

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        [Arg("stop", IsSwitch = true)]
        [Help("若启用，则取消当前已启用的日程提醒（若存在）。")]
        public bool Stop { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        [Arg("start", Default = -1)]
        [Help("提醒的相对延后时间（分钟）。")]
        public int SleepMinutes { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        [FreeArg]
        [Help("提醒的信息。")]
        public string Message { get; set; }

        private static Task _tThread;
        private static CancellationTokenSource Cts = new CancellationTokenSource();
        private static CancellationToken Ct = Cts.Token;
        private static DateTime _newTime;
        private static string _message;
        
        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            string userId = routeMsg.UserId;
            MessageType type = routeMsg.MessageType;
            Authority level = routeMsg.CurrentAuthority;
            if (type != MessageType.Private)
            {
                return routeMsg.ToSource(DefaultReply.PrivateOnly);
            }

            if (level != Authority.Root)
            {
                return routeMsg.ToSource(DefaultReply.RootOnly);
            }

            bool isTaskFree = _tThread == null || _tThread.IsCanceled || _tThread.IsCompleted;
            if (SleepMinutes > 0 && Message != null)
            {
                if (!isTaskFree)
                {
                    return routeMsg.ToSource($"日程提醒当前已有工作：\r\n{_newTime:HH:mm:ss}时将会通知你：\"{_message}\"。", true);
                }

                DateTime newTime = DateTime.Now.AddMinutes(SleepMinutes);
                _newTime = newTime;
                _message = Message;

                _tThread = Task.Run(() =>
                {
                    while (DateTime.Now < _newTime)
                    {
                        Thread.Sleep(1000);
                        Ct.ThrowIfCancellationRequested();
                    }

                    SendMessageAsync(new CoolQRouteMessage(_message, new CoolQIdentity(userId, MessageType.Private)));
                });
                string reply = $"日程提醒已新建，{_newTime:HH:mm:ss}时将会通知你：\"{_message}\"。";
                return routeMsg.ToSource(reply, true);
            }
            else if (Stop)
            {
                string reply;
                if (!isTaskFree)
                {
                    Cts.Cancel();
                    reply = $"已经取消{_newTime:HH:mm:ss}的日程提醒：\"{_message}\"";
                    _message = default;
                    _newTime = default;
                }
                else
                    reply = "当前没有日程提醒。";
                return routeMsg.ToSource(reply, true);
            }
            else return routeMsg.ToSource(DefaultReply.ParamError, true);
        }
    }
}
