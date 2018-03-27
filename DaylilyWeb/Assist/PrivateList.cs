using DaylilyWeb.Interface.CQHttp;
using DaylilyWeb.Models.CQRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DaylilyWeb.Assist
{
    public class PrivateList
    {
        public PrivateInfo this[long groupId]
        {
            get
            {
                return di[groupId];
            }
        }

        public void Add(long privateId)
        {
            if (di.Keys.Contains(privateId))
                return;
            di.Add(privateId, new PrivateInfo());
        }
        Dictionary<long, PrivateInfo> di = new Dictionary<long, PrivateInfo>();

    }
    public class PrivateInfo
    {
        public Queue<PrivateMsg> MsgQueue { get; set; } = new Queue<PrivateMsg>();
        public Thread Thread { get; set; }
        public PrivateMsg PreInfo { get; set; } = new PrivateMsg();
        public bool LockMsg { get; set; } = false;  // 用于判断是否超出消息阀值
    }
}
