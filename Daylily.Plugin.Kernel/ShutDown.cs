using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Daylily.Plugin.Kernel
{
    [Name("关闭黄花菜")]
    [Author("yf_extension")]
    [Version(2, 0, 4, PluginVersion.Beta)]
    [Help("可将黄花菜关闭一段时间，支持自定义时间。", Authority = Authority.Admin)]
    [Command("poweroff")]
    public class ShutDown : CoolQCommandPlugin
    {
        [FreeArg(Default = 10)]
        [Help("关闭的持续时间，最大支持7天，最小支持1分钟。（单位：分钟）")]
        public int ElapsingTime { get; set; }

        public override Guid Guid { get; } = new Guid("32d3ca82-969e-4757-8575-7ab11371766a");
        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            if (routeMsg.MessageType == MessageType.Private)
                return routeMsg.ToSource("你想怎么样。。");

            if (routeMsg.CurrentAuthority == Authority.Public)
            {
                return routeMsg.ToSource("请你坐下，，让你的管理员来处决我");
            }

            var time = TimeSpan.FromMinutes(ElapsingTime);
            if (time < TimeSpan.FromMinutes(1) || time > TimeSpan.FromDays(7))
            {
                return routeMsg.ToSource("时间范围设置不对，，你故意的8，，，");
            }

            var plugin = PluginManager.Current.GetPlugin<ShutdownWatcher>();
            if (plugin == null)
            {
                return routeMsg.ToSource("操作失败了。。当前未装载相关插件");
            }

            var newTime = DateTime.Now + time;
            while (plugin.IsScanning)
                Thread.Sleep(10);

            plugin.ExpireTimeCollection.Add(routeMsg.CoolQIdentity, newTime);
            plugin.SaveConfig();
            return routeMsg.ToSource($"我自闭去了。。等我{newTime:g}回来");
        }
    }
}
