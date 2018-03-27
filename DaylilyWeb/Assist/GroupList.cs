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
        public GroupInfo this[long groupId]
        {
            get
            {
                return di[groupId];
            }
        }

        public void Add(long groupID)
        {
            if (di.Keys.Contains(groupID))
                return;
            di.Add(groupID, new GroupInfo());
        }
        Dictionary<long, GroupInfo> di = new Dictionary<long, GroupInfo>();

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
