using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Daylily.Bot.Backend.Plugins;
using Daylily.Common.Logging;
using Daylily.CoolQ;
using Daylily.CoolQ.CoolQHttp;
using Daylily.CoolQ.CoolQHttp.ResponseModel.Abstract;

namespace Daylily.Plugin.Kernel
{
    class SessionUpdaterSvc : ServicePlugin
    {
        public override Guid Guid => new Guid("21a5d3c8-255c-4219-a682-2d0bcc0b5176");

        public override void OnInitialized(string[] args)
        {
            base.OnInitialized(args);

            int i = 0;
            //UpdateGroupList(i);

            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000 * 60 * 30);
                    UpdateGroupList(i);
                    i++;
                }
            });
        }

        private static void UpdateGroupList(int i)
        {
            List<GroupInfo> list = CoolQHttpApiClient.GetGroupList().Data;
            List<GroupMemberGroupInfo> newList = new List<GroupMemberGroupInfo>();
            foreach (var groupInfo in list)
            {
                try
                {
                    GroupInfoV2 v2 = CoolQHttpApiClient.GetGroupInfoV2(groupInfo.GroupId.ToString()).Data;
                    v2.Members = CoolQHttpApiClient.GetGroupMemberList(v2.GroupId.ToString()).Data/*.Where(q => q.Role == "member")*/.ToList();
                    if (i % 6 == 0)
                        foreach (var groupMember in v2.Members)
                        {
                            var user = newList.FirstOrDefault(m => m.UserId == groupMember.UserId);
                            if (user == null)
                                newList.Add(new GroupMemberGroupInfo(groupMember.UserId));
                            user = newList.FirstOrDefault(m => m.UserId == groupMember.UserId);
                            if (!user.GroupIdList.Contains(groupInfo.GroupId)) user.GroupIdList.Add(groupInfo.GroupId);
                        }
                    CoolQDispatcher.Current.SessionInfo.AddOrUpdateGroup(v2);
                    Logger.Success($"{groupInfo.GroupName} ({groupInfo.GroupId}): 管理{v2.Admins.Count}人, 成员{v2.Members.Count}人.");
                }
                catch (Exception)
                {
                    Logger.Error($"{groupInfo.GroupName} ({groupInfo.GroupId}) 加载失败。");
                }
            }

            string[] oldGroups = CoolQDispatcher.Current.SessionInfo.Sessions.Select(k => k.Value.Id).ToArray();
            string[] newGroups = list.Select(k => k.GroupId.ToString()).ToArray();
            string[] noUseGroup = oldGroups.Where(k => !newGroups.Contains(k)).ToArray();
            foreach (var groupId in noUseGroup)
            {
                try
                {
                    CoolQDispatcher.Current.SessionInfo.RemoveGroup(groupId);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            if (i % 6 == 0) CoolQDispatcher.Current.GroupMemberGroupInfo = newList;
        }
    }
}
