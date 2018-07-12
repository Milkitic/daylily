using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Daylily.Common.Assist;
using Daylily.Common.Interface.CQHttp;
using Daylily.Common.Models.CQResponse;
using Daylily.Common.Models.CQResponse.Api;

namespace Daylily.Common.Models.MessageList
{
    public class GroupList
    {
        private readonly ConcurrentDictionary<long, GroupSettings> _dicGroup = new ConcurrentDictionary<long, GroupSettings>();

        public GroupSettings this[long groupId] => _dicGroup[groupId];

        public void Add(long groupId) => _dicGroup.TryAdd(groupId, new GroupSettings(groupId.ToString()));

    }

    public class GroupSettings
    {
        private readonly object _taskLock = new object();
        public string Id { get; set; }
        public ConcurrentQueue<GroupMsg> MsgQueue { get; set; } = new ConcurrentQueue<GroupMsg>();
        public Task Task { get; set; }
        public int MsgLimit { get; set; } = 10;
        public bool LockMsg { get; set; } = false; // 用于判断是否超出消息阀值
        public GroupInfoV2 Info { get; set; }

        public GroupSettings(string groupId)
        {
            Id = groupId;
            UpdateInfo();
        }

        public bool TryRun(Action action)
        {
            bool isTaskFree;
            lock (_taskLock)
            {
                isTaskFree = Task == null || Task.IsCanceled || Task.IsCompleted;
                if (isTaskFree)
                {
                    Task = Task.Run(action);
                }
            }
            return isTaskFree;
        }

        private void UpdateInfo()
        {
            try
            {
                Info = CqApi.GetGroupInfoV2(Id).Data ?? new GroupInfoV2
                {
                    GroupName = "群" + Id,
                    GroupId = long.Parse(Id),
                    Admins = new List<GroupInfoV2Admins>()
                };
            }
            catch (Exception ex)
            {
                Info = new GroupInfoV2
                {
                    GroupName = "群" + Id,
                    GroupId = long.Parse(Id),
                    Admins = new List<GroupInfoV2Admins>()
                };
            }
        }
    }
}
