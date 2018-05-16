using Daylily.Common.Interface.CQHttp;
using Daylily.Common.Models.CQResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Daylily.Common.Assist
{
    public class GroupList
    {
        Dictionary<long, GroupSettings> dicGroup = new Dictionary<long, GroupSettings>();

        public GroupSettings this[long groupId]
        {
            get
            {
                return dicGroup[groupId];
            }
        }

        public void Add(long groupId)
        {
            if (dicGroup.Keys.Contains(groupId))
                return;

            dicGroup.Add(groupId, new GroupSettings(groupId.ToString()));
        }

    }

    public class GroupSettings
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Queue<GroupMsg> MsgQueue { get; set; } = new Queue<GroupMsg>();
        public Thread Thread { get; set; }
        public int MsgLimit { get; set; } = 10;
        public bool LockMsg { get; set; } = false;  // 用于判断是否超出消息阀值
        public List<long> AdminList { get; set; } = new List<long>();

        HttpApi CQApi = new HttpApi();

        public GroupSettings(string groupId)
        {
            Id = groupId;
            UpdateInfo();
        }

        public void UpdateInfo()
        {
            var info = CQApi.GetGroupInfo(Id);
            string name = info == null ? Id : info.GroupName;
            Name = name;

            var adminList = CQApi.GetGroupMemberList(Id);
            adminList.Data.RemoveAll(x => x.Role == "member");
            foreach(var item in adminList.Data)
            {
                AdminList.Add(item.UserId);
            }
        }
    }
}
