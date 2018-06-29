using Daylily.Common.Interface.CQHttp;
using Daylily.Common.Models.CQResponse;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Daylily.Common.Assist
{
    public class GroupList
    {
        private readonly Dictionary<long, GroupSettings> _dicGroup = new Dictionary<long, GroupSettings>();

        public GroupSettings this[long groupId] => _dicGroup[groupId];

        public void Add(long groupId)
        {
            if (_dicGroup.Keys.Contains(groupId))
                return;

            _dicGroup.Add(groupId, new GroupSettings(groupId.ToString()));
        }

    }

    public class GroupSettings
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Queue<GroupMsg> MsgQueue { get; set; } = new Queue<GroupMsg>();
        public Thread Thread { get; set; }
        public int MsgLimit { get; set; } = 10;
        public bool LockMsg { get; set; } = false; // 用于判断是否超出消息阀值
        public List<long> AdminList { get; set; } = new List<long>();

        public GroupSettings(string groupId)
        {
            Id = groupId;
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            if (Id == null) Logger.PrimaryLine("Id is null!!!!");
            try
            {
                var info = CqApi.GetGroupInfo(Id);
                string name = info == null ? Id : info.GroupName;
                Name = name;
            }
            catch
            {
                Name = "群" + Id;
            }
            
            var adminList = CqApi.GetGroupMemberList(Id);
            if (adminList.Data == null)
                Logger.PrimaryLine(Id + ": adminList.Data is null!!!!");
            else
            {
                adminList.Data.RemoveAll(x => x.Role == "member");
                foreach (var item in adminList.Data)
                {
                    AdminList.Add(item.UserId);
                }
            }
        }
    }
}
