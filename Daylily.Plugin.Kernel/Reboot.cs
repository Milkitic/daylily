using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.CoolQ;
using Daylily.CoolQ.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Daylily.Bot.Messaging;
using Daylily.CoolQ.Plugin;

namespace Daylily.Plugin.Kernel
{
    [Name("强制重启")]
    [Author("yf_extension")]
    [Version(2, 0, 1, PluginVersion.Stable)]
    [Help("此命令会立刻结束程序进程并重启。", Authority = Authority.Root)]
    [Command("reboot")]
    public class Reboot : CoolQCommandPlugin
    {
        public override Guid Guid => new Guid("ee3fc222-b05d-403b-b8c2-542a61b72a4d");
        private static RebootInfo _rebootInfo;

        public override void OnInitialized(StartupConfig startup)
        {
            _rebootInfo = LoadSettings<RebootInfo>();
        }

        public override void AllPlugins_Initialized(StartupConfig startup)
        {
            if (_rebootInfo?.OperatorId == null ||
                _rebootInfo?.RebootTime == null) return;
            var rebootTime = _rebootInfo.RebootTime.Value;
            var id = _rebootInfo.OperatorId.Value;
            Task.Run(() =>
            {
                int i = 0;
                while (DaylilyCore.Current.MessageDispatcher == null && i < 30)
                {
                    Thread.Sleep(100);
                    i++;
                }

                SendMessage(new CoolQRouteMessage(
                    "重启完成，花费" +
                    Math.Round((DateTime.Now - rebootTime).TotalSeconds, 3) +
                    "秒.", id));
            });

            _rebootInfo.OperatorId = null;
            _rebootInfo.RebootTime = null;
            SaveSettings(_rebootInfo);
        }

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            if (routeMsg.CurrentAuthority != Authority.Root)
                return routeMsg.ToSource(DefaultReply.RootOnly, true);

            _rebootInfo = new RebootInfo(DateTime.Now, routeMsg.CoolQIdentity);
            SaveSettings(_rebootInfo);
            SendMessage(routeMsg.ToSource("开始重启..."));
            Environment.Exit(0);
            return null;
        }

        private class RebootInfo
        {
            public RebootInfo()
            {

            }

            public RebootInfo(DateTime rebootTime, CoolQIdentity operatorId)
            {
                RebootTime = rebootTime;
                OperatorId = operatorId;
            }

            public DateTime? RebootTime { get; set; }
            public CoolQIdentity? OperatorId { get; set; }
        }
    }
}
