using Daylily.Common.Assist;
using Daylily.Common.Models;
using System;
using System.Threading;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Web.Function.Application.Command
{
    public class Rcon : AppConstruct
    {
        public override string Name => "日程提醒";
        public override string Author => "yf_extension";
        public override PluginVersion Version => PluginVersion.Alpha;
        public override string VersionNumber => "1.0";
        public override string Description => "建立一个日程提醒";
        public override string Command => "rcon";
        public override AppType AppType => AppType.Command;

        private static string UserId { get; set; }
        private static Thread _tThread;
        private string _message;

        public override void OnLoad(CommonMessage commonMessage)
        {
            throw new NotImplementedException();
        }

        public override CommonMessageResponse OnExecute(CommonMessage commonMsg) // 必要方法
        {
            // 发送者的QQ
            string userId = commonMsg.UserId;
            UserId = userId;

            // 包含消息种类，分别为Group, Discuss, Private，省去判断以上是否为null来判定消息种类
            MessageType type = commonMsg.MessageType;
            if (type != MessageType.Private)
            {
                return new CommonMessageResponse(LoliReply.PrivateOnly, commonMsg);
                //return null;  // 返回null则不返回消息
            }

            string parameter = commonMsg.Parameter;
            // 当前处于的权限状态，默认为Public（即开放权限）
            // 当用户执行 /sudo 或 /root 会分别触发 Admin（对应群的管理员）和 Root（系统管理员），以此做出对应权限所对应的功能
            PermissionLevel level = commonMsg.PermissionLevel;

            if (level != PermissionLevel.Root)
                return new CommonMessageResponse(LoliReply.RootOnly, commonMsg);

            // 假设做一个定时报告程序（此仅为全局共享，对于用户用途不大）
            if (string.IsNullOrEmpty(parameter))
                return new CommonMessageResponse(LoliReply.ParamError, commonMsg, true);
            string[] param = parameter.Split(" ");
            if (param.Length > 3)
                return new CommonMessageResponse(LoliReply.ParamMissing, commonMsg, true);

            if (param[0] == "start" && double.TryParse(param[1], out double sleepTime) && param[2] != null)
            {
                // 默认处理机制为单线程，返回一个对象主线程继续工作
                // 若需新建线程，则手动处理：
                if (_tThread != null && _tThread.IsAlive)
                    return new CommonMessageResponse("计时器正在工作，请先停止", commonMsg, true);

                _message = param[2];

                _tThread = new Thread(MultiThread);
                _tThread.Start(sleepTime);

                string reply = "启动了计时器，" + DateTime.Now.AddMinutes(sleepTime).ToString("HH:mm:ss") + "后会通知你：" +
                               param[2];
                // 当所用参数为(string,CommonMessage)，则自动返回给所在群（组）的所在成员（通常不用其他重载，为框架所用）
                return new CommonMessageResponse(reply, commonMsg, true);
            }

            if (param[0] != "stop" || param.Length != 1)
                return new CommonMessageResponse(LoliReply.ParamError, commonMsg, true);
            {
                if (_tThread != null && _tThread.IsAlive)
                    _tThread.Interrupt();
                string reply = "计时器已经停止";
                return new CommonMessageResponse(reply, commonMsg, true);
            }

        }

        /// <summary>
        /// 这里将无法返回消息对象，发消息需使用 SendMessage处理
        /// </summary>
        private void MultiThread(object time)
        {
            string msg = _message;
            double sleepTime = (double)time;


            // 多线程请务必做好异常处理，防止程序异常退出
            try
            {
                Thread.Sleep((int)(sleepTime * 60 * 1000));
                // 这里可以做大量其他操作，更新数据库等，不阻塞主线程
                SendMessage(new CommonMessageResponse(msg, UserId, true), null, null, MessageType.Private);
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
            }

        }
    }
}
