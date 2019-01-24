using Daylily.Bot.Backend;
using Daylily.Common.Logging;
using Daylily.CoolQ;
using Daylily.CoolQ.CoolQHttp.ResponseModel.Report;
using Daylily.CoolQ.Messaging;
using System;
using System.Threading;
using Daylily.Bot.Messaging;
using Daylily.CoolQ.Plugin;

namespace Daylily.Plugin.Basic
{
    // 激活插件的命令，支持多字符串
    [Command("demo1")]
    // 插件名
    [Name("测试Demo插件")]
    // 插件作者
    [Author("yf_extension")]
    // 插件版本
    [Version(0, 1, 0, PluginVersion.Stable)]
    // 插件说明，用于help查询
    [Help("用于对于插件开发进行Demo演示")]
    class Demo : CoolQCommandPlugin // 继承此类做为命令，此外还有其他两种类型
    {
        public override Guid Guid => new Guid("b5c867e1-0d3d-4efb-ad76-91c3a76100f8");

        private static Thread _tThread;
        private static string UserId { get; set; }
        
        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            // 接收的信息
            string message = routeMsg.RawMessage;
            // 发送者的QQ
            string userId = routeMsg.UserId;
            UserId = userId;
            // 发送者所在群ID（若是私聊或讨论组则为null）
            string groupId = routeMsg.GroupId;
            // 发送者所在讨论组ID（若是私聊或群则为null）
            string discussId = routeMsg.DiscussId;
            // 包含消息种类，分别为Group, Discuss, Private，省去判断以上是否为null来判定消息种类
            MessageType type = routeMsg.MessageType;
            if (type != MessageType.Private)
            {
                Logger.Error("不能非私聊");
                //return null;  // 返回null则不返回消息
            }

            // 接收的信息的Id，用于撤回等操作
            long msgId = routeMsg.MessageId;

            // 若是命令，下面两个字段不为null，当接收消息是 "/demo asdf 1234" 的情况：
            // 此字段为 "demo"
            string command = routeMsg.CommandName;
            // 此字段为 "asdf 1234"
            string argString = routeMsg.ArgString;
            // 当前处于的权限状态，默认为Public（即开放权限）
            // 当用户执行 /sudo 或 /root 会分别触发 Admin（对应群的管理员）和 Root（系统管理员），以此做出对应权限所对应的功能
            Authority level = routeMsg.CurrentAuthority;
            if (level == Authority.Public)
                Logger.Info("当前所用权限：Public");
            else if (level == Authority.Admin)
                Logger.Info("当前所用权限：Admin");
            else if (level == Authority.Root)
                Logger.Info("当前所用权限：Root");

            // 暂无实际用处，当前为框架所用，后续会有变动
            string fullcmd = routeMsg.FullCommand;
            // 包含json所传原生参数，通常只有少数情况会使用（获取字体，发送时间，匿名情况等）
            CoolQPrivateMessageApi privateObj = routeMsg.Private;
            CoolQDiscussMessageApi discussObj = routeMsg.Discuss;
            CoolQGroupMessageApi groupObj = routeMsg.Group;

            // 假设做一个定时报告程序（此仅为全局共享，对于用户用途不大）
            if (string.IsNullOrEmpty(argString))
                return routeMsg.ToSource("请填写参数", enableAt: true);
            string[] param = argString.Split(" ");
            if (param.Length > 2)
                return routeMsg.ToSource("参数不正确", enableAt: true);

            if (param[0] == "start" && int.TryParse(param[1], out int sleepTime))
            {
                // 默认处理机制为单线程，返回一个对象主线程继续工作
                // 若需新建线程，则手动处理：
                if (_tThread != null && _tThread.IsAlive)
                    return routeMsg.ToSource("计时器正在工作，请先停止", enableAt: true);

                _tThread = new Thread(new ParameterizedThreadStart(MultiThread));
                _tThread.Start(sleepTime);

                string reply = "启动了计时器";
                // 当所用参数为(string,CommonMessage)，则自动返回给所在群（组）的所在成员（通常不用其他重载，为框架所用）
                return routeMsg.ToSource(reply, enableAt: true);
            }
            else if (param[0] == "stop" && param.Length == 1)
            {
                if (_tThread != null && _tThread.IsAlive)
                    _tThread.Abort();
                string reply = "计时器已经停止";
                return routeMsg.ToSource(reply, enableAt: true);
            }
            else
                return routeMsg.ToSource("参数不正确", enableAt: true);

            // 若需回复至别处，需以下实现
            string reply2 = "回复到了别处";
            string userId2 = "xxxxxxx";
            long groupId2 = 123456;
            SendMessage(new CoolQRouteMessage(reply2, new CoolQIdentity(groupId2, MessageType.Group), userId2));
            return null;
        }

        /// <summary>
        /// 这里将无法返回消息对象，发消息需使用 SendMessage处理
        /// </summary>
        private void MultiThread(object time)
        {
            int sleepTime = (int)time;
            while (true)
            {
                Thread.Sleep(sleepTime);

                // 多线程请务必做好异常处理，防止程序异常退出
                try
                {
                    // 这里可以做大量其他操作，更新数据库等，不阻塞主线程
                    string message = "Pong!";
                    SendMessage(new CoolQRouteMessage(message, new CoolQIdentity(UserId, MessageType.Private)));
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }
        }
    }
}
