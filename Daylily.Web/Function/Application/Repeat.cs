using Daylily.Common.Assist;
using Daylily.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Web.Function.Application
{
    public class Repeat : AppConstruct
    {
        public override string Name => "复读";
        public override string Author => "yf_extension";
        public override PluginVersion Version => PluginVersion.Stable;
        public override string VersionNumber => "1.0";
        public override string Description => "按一定条件触发复读";
        public override string Command => null;
        public override AppType AppType => AppType.Application;

        private static readonly Dictionary<string, GroupSettings> GroupDic = new Dictionary<string, GroupSettings>();
        private const int MaxNum = 10;

        public override void OnLoad(string[] args)
        {
            //throw new NotImplementedException();
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
                    GroupId = groupId,
                });
                GroupDic[groupId].Thread = new Thread(delegate ()
                {
                    while (true)
                    {
                        Thread.Sleep(Rnd.Next(1000, 10000));
                        if (GroupDic[groupId].IntQueue <= 0)
                        {
                            if (GroupDic[groupId].Locked)
                            {
                                GroupDic[groupId].Locked = false;
                                Logger.DebugLine(groupId + " unlocked");
                            }
                            continue;
                        }
                        if (Rnd.NextDouble() < 0.02)
                            Thread.Sleep(Rnd.Next(30000, 45000));

                        GroupDic[groupId].IntQueue--;
                        Logger.DebugLine(groupId + " decresed to " + GroupDic[groupId].IntQueue);
                    }

                });
                GroupDic[groupId].Thread.Start();
            }

            if (GroupDic[groupId].IntQueue >= MaxNum && !GroupDic[groupId].Locked)
            {
                GroupDic[groupId].Locked = true;
                Logger.DebugLine(groupId + " locked");
                Logger.SuccessLine(groupId + "的" + messageObj.UserId + "触发了复读");
                Thread.Sleep(Rnd.Next(1000, 8000));
                return new CommonMessageResponse(messageObj.Message, messageObj);
            }


            GroupDic[groupId].IntQueue++;
            Logger.DebugLine(groupId + " incresed to " + GroupDic[groupId].IntQueue);
            return null;
        }

        private class GroupSettings
        {
            public string GroupId { get; set; }
            public int IntQueue { get; set; }
            public Thread Thread { get; set; }
            public bool Locked { get; set; } = false;
        }

    }
}
