using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Daylily.Common.Assist;
using Daylily.Common.Models;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;
using Daylily.Common.Utils;
using Newtonsoft.Json;

namespace Daylily.Common.Function.Application
{
    public class GroupQuiet : AppConstruct
    {
        public override string Name => "死群发熊猫";
        public override string Author => "yf_extension";
        public override PluginVersion Version => PluginVersion.Stable;
        public override string VersionNumber => "1.0";
        public override string Description => "群长时间不说话时，发熊猫";
        public override string Command => null;
        public override AppType AppType => AppType.Application;
        private static readonly string PandaDir = Path.Combine(Domain.CurrentDirectory, "panda");
        private static ConcurrentDictionary<string, GroupSettings> _groupDic;
        public override void OnLoad(string[] args)
        {
            Logger.WriteLine("上次群发言情况载入中。");
            _groupDic = LoadSettings<ConcurrentDictionary<string, GroupSettings>>();
            if (_groupDic != null)
            {
                foreach (var item in _groupDic)
                {
                    item.Value.Thread = new Thread(DelayScan);
                    item.Value.Thread.Start(item.Key);
                }
            }
            else _groupDic = new ConcurrentDictionary<string, GroupSettings>();
            Logger.WriteLine("上次群发言情载入完毕，并开启了线程。");
        }

        public override CommonMessageResponse OnExecute(in CommonMessage messageObj)
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
                    CdTime = 60 * 60 * 9,
                    //CdTime = 15,
                });

                _groupDic[groupId].Thread = new Thread(DelayScan);
                _groupDic[groupId].Thread.Start(groupId);
            }

            if ((DateTime.Now - _groupDic[groupId].StartCd).TotalSeconds > _groupDic[groupId].CdTime)
            {
                _groupDic[groupId].LastSent = DateTime.Now;
                _groupDic[groupId].LastSentIsMe = false;
                //GroupDic[groupId].TrigTime = Rnd.Next(4, 5);
                _groupDic[groupId].TrigTime = Rnd.Next(60 * 60 * 2, 60 * 60 * 3);
                Logger.DebugLine(groupId + ". Last: " + _groupDic[groupId].LastSent + ", Sent: " + _groupDic[groupId].LastSentIsMe);
                SaveSettings(_groupDic);
            }
            else
                Logger.DebugLine(groupId + ". CD");
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
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }
        private class GroupSettings
        {
            public CommonMessage MessageObj { get; set; }
            [JsonIgnore]
            public Thread Thread { get; set; }
            public bool LastSentIsMe { get; set; }
            public DateTime LastSent { get; set; }
            public DateTime StartCd { get; set; }
            public long TrigTime { get; set; } //seconds
            public long CdTime { get; set; } //seconds

        }
    }
}
