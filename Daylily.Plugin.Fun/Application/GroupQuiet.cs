using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.Common;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Daylily.Plugin.ShaDiao.Application
{
    [Name("死群熊猫")]
    [Author("yf_extension")]
    [Version(2, 0, 3, PluginVersion.Stable)]
    [Help("群内长时间无人发言发一张相关的熊猫。")]
    public sealed class GroupQuiet : CoolQApplicationPlugin
    {
        public override Guid Guid => new Guid("5f60b007-7984-4eae-98e5-bdfb4cfc9df9");

        private static readonly string PandaDir = Path.Combine(Domain.ResourcePath, "panda");
        private static CoolQIdentityDictionary<GroupSettings> groupSettings;

        public GroupQuiet()
        {
            Logger.Origin("上次群发言情况载入中。");
            groupSettings = LoadSettings<CoolQIdentityDictionary<GroupSettings>>();
            if (groupSettings != null)
            {
                groupSettings.Foreach(settings => { settings.Value.Task = Task.Run(() => DelayScan(settings.Identity)); });
            }
            else groupSettings = new CoolQIdentityDictionary<GroupSettings>();
            Logger.Origin("上次群发言情载入完毕，并开启了线程。");
        }

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            if (routeMsg.MessageType == MessageType.Private)
                return null;
            var id = (CoolQIdentity)routeMsg.Identity;

            if (!groupSettings.ContainsKey(id))
            {
                groupSettings.Add(id, new GroupSettings
                {
                    LastSentIsMe = false,
                    CdTime = 60 * 60 * 24,
                });

                groupSettings[id].Task = Task.Run(() => DelayScan(id));
            }

            if ((DateTime.Now - groupSettings[id].StartCd).TotalSeconds > groupSettings[id].CdTime)
            {
                groupSettings[id].LastSent = DateTime.Now;
                groupSettings[id].LastSentIsMe = false;
                groupSettings[id].TrigTime = StaticRandom.Next(60 * 60 * 2, 60 * 60 * 3);
#if DEBUG
                //Logger.Debug(groupId + ". Last: " + _groupDic[groupId].LastSent + ", Sent: " + _groupDic[groupId].LastSentIsMe);
#endif
                SaveSettings(groupSettings);
            }
            else
            {
#if DEBUG
                //Logger.Debug(groupId + ". CD");
#endif
            }
            return null;
        }
        private void DelayScan(CoolQIdentity id)
        {
            while (true)
            {
                Thread.Sleep(5000);
                if (groupSettings[id].LastSentIsMe) continue;
                if ((DateTime.Now - groupSettings[id].LastSent).TotalSeconds < groupSettings[id].TrigTime) continue;
                groupSettings[id].LastSentIsMe = true;
                groupSettings[id].StartCd = DateTime.Now;
                try
                {
                    var cqImg = new FileImage(Path.Combine(PandaDir, "quiet.jpg")).ToString();
                    SendMessage(new CoolQRouteMessage(cqImg, id));
                    SaveSettings(groupSettings);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }
        }
        private class GroupSettings
        {
            [JsonIgnore]
            public Task Task { get; set; }
            public bool LastSentIsMe { get; set; }
            public DateTime LastSent { get; set; }
            public DateTime StartCd { get; set; }
            public long TrigTime { get; set; } //seconds
            public long CdTime { get; set; } //seconds

        }
    }
}
