using System;
using System.Threading;
using System.Threading.Tasks;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;

namespace Daylily.Plugin.Core.Command
{
    [Name("日程提醒")]
    [Author("yf_extension")]
    [Version(0, 1, 1, PluginVersion.Alpha)]
    [Help("日程提醒管理。", HelpType = PermissionLevel.Root)]
    [Command("rcon")]
    public class Rcon : CommandPlugin
    {
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
        
        public override void Initialize(string[] args) { }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            string userId = messageObj.UserId;
            MessageType type = messageObj.MessageType;
            PermissionLevel level = messageObj.PermissionLevel;
            if (type != MessageType.Private)
            {
                return new CommonMessageResponse(LoliReply.PrivateOnly, messageObj);
            }

            if (level != PermissionLevel.Root)
            {
                return new CommonMessageResponse(LoliReply.RootOnly, messageObj);
            }

            bool isTaskFree = _tThread == null || _tThread.IsCanceled || _tThread.IsCompleted;
            if (SleepMinutes > 0 && Message != null)
            {
                if (!isTaskFree)
                {
                    return new CommonMessageResponse($"日程提醒当前已有工作：\r\n{_newTime:HH:mm:ss}时将会通知你：\"{_message}\"。", messageObj, true);
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

                    SendMessage(new CommonMessageResponse(_message, userId, true), null, null, MessageType.Private);
                });
                string reply = $"日程提醒已新建，{_newTime:HH:mm:ss}时将会通知你：\"{_message}\"。";
                return new CommonMessageResponse(reply, messageObj, true);
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
                return new CommonMessageResponse(reply, messageObj, true);
            }
            else return new CommonMessageResponse(LoliReply.ParamError, messageObj, true);
        }
    }
}
