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
        public Queue<GroupMsg> MsgQueue = new Queue<GroupMsg>();
        public Thread Thread;
        public GroupMsg preInfo = new GroupMsg();
        public string preString = "";
    }
}
