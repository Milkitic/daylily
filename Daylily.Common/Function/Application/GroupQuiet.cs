using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Daylily.Common.Assist;
using Daylily.Common.Models;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

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

        private static readonly string PandaDir = Path.Combine(Environment.CurrentDirectory, "panda");
        private static readonly Dictionary<string, GroupSettings> GroupDic = new Dictionary<string, GroupSettings>();
        public override void OnLoad(string[] args)
        {
        }

        public override CommonMessageResponse OnExecute(CommonMessage messageObj)
        {
            if (messageObj.MessageType == MessageType.Private)
                return null;
            string groupId = messageObj.GroupId ?? messageObj.DiscussId;

            if (!GroupDic.ContainsKey(groupId))
            {
                GroupDic.Add(groupId, new GroupSettings
                {
                    MessageObj = messageObj,
                    LastSentIsMe = false,
                    CdTime = 60 * 60 * 9,
                    //CdTime = 15,
                });

                GroupDic[groupId].Thread = new Thread(DelayScan);
                GroupDic[groupId].Thread.Start();
            }

            if ((DateTime.Now - GroupDic[groupId].StartCd).TotalSeconds > GroupDic[groupId].CdTime)
            {
                GroupDic[groupId].LastSent = DateTime.Now;
                GroupDic[groupId].LastSentIsMe = false;
                //GroupDic[groupId].TrigTime = Rnd.Next(4, 5);
                GroupDic[groupId].TrigTime = Rnd.Next(60 * 60 * 2, 60 * 60 * 3);
                Logger.DebugLine(groupId + ". Last: " + GroupDic[groupId].LastSent + ", Sent: " + GroupDic[groupId].LastSentIsMe);
            }
            else
                Logger.DebugLine(groupId + ". CD");
            return null;

            void DelayScan()
            {
                while (true)
                {
                    Thread.Sleep(5000);
                    if (GroupDic[groupId].LastSentIsMe) continue;
                    if ((DateTime.Now - GroupDic[groupId].LastSent).TotalSeconds < GroupDic[groupId].TrigTime) continue;
                    GroupDic[groupId].LastSentIsMe = true;
                    GroupDic[groupId].StartCd = DateTime.Now;
                    var resp = CqCode.EncodeFileToBase64(Path.Combine(PandaDir, "quiet.jpg"));
                    SendMessage(new CommonMessageResponse(resp, GroupDic[groupId].MessageObj));

                }
            }
        }

        private class GroupSettings
        {
            public CommonMessage MessageObj { get; set; }
            public Thread Thread { get; set; }
            public bool LastSentIsMe { get; set; }
            public DateTime LastSent { get; set; }
            public DateTime StartCd { get; set; }
            public long TrigTime { get; set; } //seconds
            public long CdTime { get; set; } //seconds

        }
    }
}
