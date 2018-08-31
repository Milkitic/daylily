using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Common;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ;
using Newtonsoft.Json;

namespace Daylily.Plugin.ShaDiao.Application
{
    [Name("死群熊猫")]
    [Author("yf_extension")]
    [Version(0, 0, 2, PluginVersion.Stable)]
    [Help("群内长时间无人发言发一张相关的熊猫。")]
    public class GroupQuiet : ApplicationPlugin
    {
        private static readonly string PandaDir = Path.Combine(Domain.ResourcePath, "panda");
        private static ConcurrentDictionary<string, GroupSettings> _groupDic;

        public GroupQuiet()
        {
            Logger.Origin("上次群发言情况载入中。");
            _groupDic = LoadSettings<ConcurrentDictionary<string, GroupSettings>>();
            if (_groupDic != null)
            {
                foreach (var item in _groupDic)
                {
                    item.Value.Task = Task.Run(() => DelayScan(item.Key));
                }
            }
            else _groupDic = new ConcurrentDictionary<string, GroupSettings>();
            Logger.Origin("上次群发言情载入完毕，并开启了线程。");
        }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            if (messageObj.MessageType == MessageType.Private)
                return null;
            string groupId = messageObj.GroupId ?? messageObj.DiscussId;

            if (!_groupDic.ContainsKey(groupId))
            {
                _groupDic.GetOrAdd(groupId, new GroupSettings
                {
                    MessageObj = messageObj,
                    LastSentIsMe = false,
                    CdTime = 60 * 60 * 24,
                });

                _groupDic[groupId].Task = Task.Run(() => DelayScan(groupId));
            }

            if ((DateTime.Now - _groupDic[groupId].StartCd).TotalSeconds > _groupDic[groupId].CdTime)
            {
                _groupDic[groupId].LastSent = DateTime.Now;
                _groupDic[groupId].LastSentIsMe = false;
                _groupDic[groupId].TrigTime = Rnd.Next(60 * 60 * 2, 60 * 60 * 3);
#if DEBUG
                Logger.Debug(groupId + ". Last: " + _groupDic[groupId].LastSent + ", Sent: " + _groupDic[groupId].LastSentIsMe);
#endif
                SaveSettings(_groupDic);
            }
            else
            {
#if DEBUG
                Logger.Debug(groupId + ". CD");
#endif
            }
            return null;
        }
        private void DelayScan(object groupIdObj)
        {
            string groupId = (string)groupIdObj;
            while (true)
            {
                Thread.Sleep(5000);
                if (_groupDic[groupId].LastSentIsMe) continue;
                if ((DateTime.Now - _groupDic[groupId].LastSent).TotalSeconds < _groupDic[groupId].TrigTime) continue;
                _groupDic[groupId].LastSentIsMe = true;
                _groupDic[groupId].StartCd = DateTime.Now;
                try
                {
                    var cqImg = new FileImage(Path.Combine(PandaDir, "quiet.jpg")).ToString();
                    SendMessage(new CommonMessageResponse(cqImg, _groupDic[groupId].MessageObj));
                    SaveSettings(_groupDic);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }
        }
        private class GroupSettings
        {
            public CommonMessage MessageObj { get; set; }
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
