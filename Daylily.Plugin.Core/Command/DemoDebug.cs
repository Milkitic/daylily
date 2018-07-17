using System;
using System.Threading;
using System.Threading.Tasks;
using Daylily.Common.Assist;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.CQResponse;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;
using Daylily.Common.Utils;
using Daylily.Common.Utils.LogUtils;

namespace Daylily.Plugin.Core.Command
{
    // 激活插件的命令，支持多字符串
    [Command("demo2")]
    // 插件名
    [Name("测试Demo插件")]
    // 插件作者
    [Author("yf_extension")]
    // 插件版本
    [Version(0, 0, 1, PluginVersion.Stable)]
    // 插件说明，用于help查询
    [Help("用于对于插件开发进行Demo演示")]
    class DemoDebug : CommandApp // 继承此类做为命令，此外还有其他两种类型
    {
        private static Thread _tThread;
        private static string UserId { get; set; }

        // 若标记此特性，则作为参数，上层会自动解析并动态赋值
        // /demo -start 123 或 /demo -stop
        [Arg("start")]
        public int Start { get; set; }
        [Arg("stop", IsSwitch = true)]
        public bool Stop { get; set; }

        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            if (Stop)
            {
                if (_tThread != null && _tThread.IsAlive)
                    _tThread.Abort();
                const string reply = "计时器已经停止";
                return new CommonMessageResponse(reply, messageObj, enableAt: true);
            }

            if (Start != default)
            {
                // 默认处理机制为单线程，返回一个对象主线程继续工作
                // 若需新建线程，则手动处理：
                if (_tThread != null && _tThread.IsAlive)
                    return new CommonMessageResponse("计时器正在工作，请先停止", messageObj, enableAt: true);

                _tThread = new Thread(new ParameterizedThreadStart(MultiThread));
                _tThread.Start(Start);

                string reply = "启动了计时器";
                // 当所用参数为(string,CommonMessage)，则自动返回给所在群（组）的所在成员（通常不用其他重载，为框架所用）
                return new CommonMessageResponse(reply, messageObj, enableAt: true);
            }
            else
                return new CommonMessageResponse("参数不正确", messageObj, enableAt: true);
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
                    Logger.Exception(ex);
                }
            }
        }
    }
}
