using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.CoolQ;
using Daylily.CoolQ.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Daylily.Bot.Messaging;
using Daylily.CoolQ.Plugin;

namespace Daylily.Plugin.Kernel
{
    [Help(Authority = Authority.Admin)]
    public class ShutdownWatcherApp : CoolQApplicationPlugin
    {
        public override MiddlewareConfig MiddlewareConfig => new BackendConfig
        {
            CanDisabled = false,
            Priority = 98
        };

        public override bool RunInMultiThreading => false;

        public override Guid Guid { get; } = new Guid("f8374b24-cf9a-4ec8-8139-b80e71f99057");

        public CoolQIdentityDictionary<DateTime?> ExpireTimeCollection { get; private set; }
        public bool IsScanning { get; set; }
        public override void OnInitialized(StartupConfig startup)
        {
            ExpireTimeCollection = LoadSettings<CoolQIdentityDictionary<DateTime?>>() ??
                                   new CoolQIdentityDictionary<DateTime?>();
            Task.Run(() =>
            {
                while (true)
                {
                    IsScanning = true;
                    try
                    {
                        ExpireTimeCollection.Foreach(pair =>
                        {
                            if (DateTime.Now < pair.Value) return;
                            CoolQDispatcher.Current.SendMessageAsync(new CoolQRouteMessage("啊，活过来了", pair.Identity));
                            pair.Value = null;
                            ExpireTimeCollection.Remove(pair.Identity);
                            SaveConfig();
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        SaveConfig();
                        // ignored
                    }

                    IsScanning = false;
                    Thread.Sleep(10000);
                }

            });
        }

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            if (routeMsg.MessageType == MessageType.Private)
                return null;
            var auth = PermissionCheckerApp.GetActualAuthority(routeMsg, out var fullCommand);
            if (fullCommand == "poweron")
            {
                bool alive = IsMeAlive(routeMsg.CoolQIdentity);
                if (alive) return routeMsg.ToSource("我还活着呢，你想干什么压");
                if (auth == Authority.Public)
                {
                    return routeMsg.ToSource("若想开机，请联系管理员使用此命令").Handle();
                }

                while (IsScanning)
                    Thread.Sleep(10);
                ExpireTimeCollection[routeMsg.CoolQIdentity] = null;
                ExpireTimeCollection.Remove(routeMsg.CoolQIdentity);
                SaveConfig();
                return routeMsg.ToSource("啊，活过来了");
            }

            if (routeMsg.RawMessage.Contains("/poweroff") || routeMsg.RawMessage.Contains("黄花菜"))
            {
                bool alive = IsMeAlive(routeMsg.CoolQIdentity);
                if (!alive)
                {
                    return routeMsg.ToSource("黄花菜当前是关机状态哦，若想开机请联系管理员使用/poweron命令").Handle();
                }
            }

            return IsMeAlive(routeMsg.CoolQIdentity)
                ? null
                : new CoolQRouteMessage().Handle();
        }

        private bool IsMeAlive(CoolQIdentity id)
        {
            if (!ExpireTimeCollection.ContainsKey(id))
                return true;

            var time = ExpireTimeCollection[id];
            if (!time.HasValue)
                return true;
            return false;
        }

        public void SaveConfig()
        {
            SaveSettings(ExpireTimeCollection);
        }
    }
}
