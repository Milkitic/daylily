using DaylilyWeb.Interface.CQHttp;
using DaylilyWeb.Models.CQResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DaylilyWeb.Assist
{
    public class GroupList
    {
        Dictionary<long, GroupSettings> dicGroup = new Dictionary<long, GroupSettings>();
        HttpApi CQApi = new HttpApi();

        public GroupSettings this[long groupId]
        {
            get
            {
                return dicGroup[groupId];
            }
        }

        public void Add(long groupID)
        {
            if (dicGroup.Keys.Contains(groupID))
                return;
            var abc = CQApi.GetGroupInfo(groupID.ToString());
            string name = abc == null ? groupID.ToString() : abc.GroupName;
            dicGroup.Add(groupID, new GroupSettings()
            {
                Name = name
            });
        }

    }

    public class GroupSettings
    {
        public string Name { get; set; }
        public Queue<GroupMsg> MsgQueue { get; set; } = new Queue<GroupMsg>();
        public Thread Thread { get; set; }
        public int MsgLimit { get; set; } = 10;
        public bool LockMsg { get; set; } = false;  // 用于判断是否超出消息阀值
    }
}
