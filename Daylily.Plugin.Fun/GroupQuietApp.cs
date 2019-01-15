using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.Common;
using Daylily.Common.Logging;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Daylily.Bot;

namespace Daylily.Plugin.Fun
{
    [Name("死群熊猫")]
    [Author("yf_extension")]
    [Version(2, 1, 0, PluginVersion.Stable)]
    [Help("群内长时间无人发言发一张相关的熊猫。")]
    public sealed class GroupQuietApp : CoolQApplicationPlugin
    {
        public override Guid Guid => new Guid("5f60b007-7984-4eae-98e5-bdfb4cfc9df9");

        private static readonly string PandaDir = Path.Combine(Domain.ResourcePath, "panda");
        private static CoolQIdentityDictionary<GroupSettings> _settings;
        private static readonly CancellationTokenSource Cts = new CancellationTokenSource();

        public override void OnInitialized(StartupConfig startup)
        {
            _settings = LoadSettings<CoolQIdentityDictionary<GroupSettings>>()
                            ?? new CoolQIdentityDictionary<GroupSettings>();
            Task.Factory.StartNew(ScanningAction, TaskCreationOptions.LongRunning);
            Logger.Origin("上次群发言情载入完毕，开启扫描。");
        }

        private void ScanningAction()
        {
            while (!Cts.IsCancellationRequested)
            {
                Thread.Sleep(1000 * 30);
                _settings.Foreach(pair =>
                {
                    var id = pair.Identity;
                    var settings = pair.Value;

                    if (settings.LastSentIsMe) return;

                    var elapsedTime = (DateTime.Now - settings.LastSent);
                    if (elapsedTime < settings.TrigTime) return;

                    settings.LastSentIsMe = true;
                    settings.StartCd = DateTime.Now;
                    settings.TrigTime = TimeSpan.FromSeconds(StaticRandom.Next(60 * 60 * 2, 60 * 60 * 3));
                    if (elapsedTime + TimeSpan.FromMinutes(2) >= settings.TrigTime) return;

                    var file = Path.Combine(PandaDir, "quiet.jpg");
                    if (File.Exists(file))
                        SendMessage(new CoolQRouteMessage(new FileImage(file), id));
                    else
                        OnErrorOccured(
                            new ExceptionEventArgs(new FileNotFoundException("Cannot locate file", file)));

                    SaveSettings(_settings);
                });
            }
        }

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            if (routeMsg.MessageType == MessageType.Private)
                return null;
            var id = (CoolQIdentity)routeMsg.Identity;

            if (!_settings.ContainsKey(id))
            {
                _settings.Add(id, new GroupSettings
                {
                    LastSentIsMe = false,
                    CdTime = TimeSpan.FromHours(24),
                });
            }

            if (DateTime.Now - _settings[id].StartCd > _settings[id].CdTime)
            {
                _settings[id].LastSent = DateTime.Now;
                _settings[id].LastSentIsMe = false;
#if DEBUG
                //Logger.Debug(groupId + ". Last: " + _groupDic[groupId].LastSent + ", Sent: " + _groupDic[groupId].LastSentIsMe);
#endif
                SaveSettings(_settings);
            }
            else
            {
#if DEBUG
                //Logger.Debug(groupId + ". CD");
#endif
            }
            return null;
        }

        private class GroupSettings
        {
            [JsonProperty("freeze")]
            public bool LastSentIsMe { get; set; }
            [JsonProperty("last")]
            public DateTime LastSent { get; set; }
            [JsonProperty("cd_last")]
            public DateTime StartCd { get; set; }
            [JsonProperty("trig")]
            public TimeSpan TrigTime { get; set; }
            [JsonProperty("cd")]
            public TimeSpan CdTime { get; set; }
        }
    }
}
