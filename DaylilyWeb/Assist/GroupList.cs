using DaylilyWeb.Interface.CQHttp;
using DaylilyWeb.Models.CQRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DaylilyWeb.Assist
{
    public class GroupList
    {
        Dictionary<long, GroupInfo> dicGroup = new Dictionary<long, GroupInfo>();

        public GroupInfo this[long groupId]
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
            dicGroup.Add(groupID, new GroupInfo());
        }
    }

    public class GroupInfo
    {
        public Queue<GroupMsg> MsgQueue { get; set; } = new Queue<GroupMsg>();
        public Thread Thread { get; set; }
        public GroupMsg PreInfo { get; set; } = new GroupMsg();
        public string PreString { get; set; } = "";
        public int MsgLimit { get; set; } = 10;
        public bool LockMsg { get; set; } = false;  // 用于判断是否超出消息阀值
    }
}
