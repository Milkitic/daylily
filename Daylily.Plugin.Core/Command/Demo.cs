using System;
using System.Threading;
using Daylily.Common.Assist;
using Daylily.Common.Models;
using Daylily.Common.Models.CQResponse;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Plugin.Core.Command
{
    class Demo : AppConstruct // 继承此类，详细可查看Daylily.Common.Models.AppConstruct类
    {
        public override string Name => "测试Demo插件";
        public override string Author => "yf_extension";
        public override PluginVersion Version => PluginVersion.Stable;
        public override string VersionNumber => "1.0";
        public override string Description => "用于对于插件开发进行Demo演示";
        public override string Command => null;
        public override AppType AppType => AppType.Command;

        static Thread _tThread;
        static string UserId { get; set; }

        public override void OnLoad(CommonMessage commonMessage)
        {
            throw new NotImplementedException();
        }

        public override CommonMessageResponse OnExecute(CommonMessage commonMsg) // 必要方法
        {
            // 接收的信息
            string message = commonMsg.Message;
            // 发送者的QQ
            string userId = commonMsg.UserId;
            UserId = userId;
            // 发送者所在群ID（若是私聊或讨论组则为null）
            string groupId = commonMsg.GroupId;
            // 发送者所在讨论组ID（若是私聊或群则为null）
            string discussId = commonMsg.DiscussId;
            // 包含消息种类，分别为Group, Discuss, Private，省去判断以上是否为null来判定消息种类
            MessageType type = commonMsg.MessageType;
            if (type != MessageType.Private)
            {
                Logger.DangerLine("不能非私聊");
                //return null;  // 返回null则不返回消息
            }

            // 接收的信息的Id，用于撤回等操作
            long msgId = commonMsg.MessageId;

            // 若是命令，则需在配置文件(plugins.json)中配置映射关系(demo->Demo)（前者为用户输入，后者为类名），下面两个字段不为null，当接收消息是 "/demo asdf 1234" 的情况：
            // 此字段为 "demo"
            string command = commonMsg.Command;
            // 此字段为 "asdf 1234"
            string parameter = commonMsg.Parameter;
            // 当前处于的权限状态，默认为Public（即开放权限）
            // 当用户执行 /sudo 或 /root 会分别触发 Admin（对应群的管理员）和 Root（系统管理员），以此做出对应权限所对应的功能
            PermissionLevel level = commonMsg.PermissionLevel;
            if (level == PermissionLevel.Public)
                Logger.InfoLine("当前所用权限：Public");
            else if (level == PermissionLevel.Admin)
                Logger.InfoLine("当前所用权限：Admin");
            else if (level == PermissionLevel.Root)
                Logger.InfoLine("当前所用权限：Root");

            // 暂无实际用处，当前为框架所用，后续会有变动
            string fullcmd = commonMsg.FullCommand;
            // 包含json所传原生参数，通常只有少数情况会使用（获取字体，发送时间，匿名情况等）
            PrivateMsg privateObj = commonMsg.Private;
            DiscussMsg discussObj = commonMsg.Discuss;
            GroupMsg groupObj = commonMsg.Group;

            // 假设做一个定时报告程序（此仅为全局共享，对于用户用途不大）
            if (string.IsNullOrEmpty(parameter))
                return new CommonMessageResponse("请填写参数", commonMsg, enableAt: true);
            string[] param = parameter.Split(" ");
            if (param.Length > 2)
                return new CommonMessageResponse("参数不正确", commonMsg, enableAt: true);

            if (param[0] == "start" && int.TryParse(param[1], out int sleepTime))
            {
                // 默认处理机制为单线程，返回一个对象主线程继续工作
                // 若需新建线程，则手动处理：
                if (_tThread != null && _tThread.IsAlive)
                    return new CommonMessageResponse("计时器正在工作，请先停止", commonMsg, enableAt: true);

                _tThread = new Thread(new ParameterizedThreadStart(MultiThread));
                _tThread.Start(sleepTime);

                string reply = "启动了计时器";
                // 当所用参数为(string,CommonMessage)，则自动返回给所在群（组）的所在成员（通常不用其他重载，为框架所用）
                return new CommonMessageResponse(reply, commonMsg, enableAt: true);
            }
            else if (param[0] == "stop" && param.Length == 1)
            {
                if (_tThread != null && _tThread.IsAlive)
                    _tThread.Abort();
                string reply = "计时器已经停止";
                return new CommonMessageResponse(reply, commonMsg, enableAt: true);
            }
            else
                return new CommonMessageResponse("参数不正确", commonMsg, enableAt: true);

            // 若需回复至别处，需以下实现
            string reply2 = "回复到了别处";
            string userId2 = "xxxxxxx";
            string groupId2 = "xxxxxxx";
            string discussId2 = "xxxxxxx";
            MessageType type2 = MessageType.Group;
            SendMessage(new CommonMessageResponse(reply2, userId2, true), groupId2, discussId2, type2);
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
                    SendMessage(new CommonMessageResponse(message, UserId, true), null, null, MessageType.Private);
                }
                catch (Exception ex)
                {
                    Logger.WriteException(ex);
                }
            }
        }
    }
}
