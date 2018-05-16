using Daylily.Common.Interface.CQHttp;
using Daylily.Common.Models.CQResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Daylily.Common.Assist
{
    public class PrivateList
    {
        public PrivateSettings this[long groupId]
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
            di.Add(privateId, new PrivateSettings());
        }
        Dictionary<long, PrivateSettings> di = new Dictionary<long, PrivateSettings>();

    }
    public class PrivateSettings
    {
        public Queue<PrivateMsg> MsgQueue { get; set; } = new Queue<PrivateMsg>();
        public Thread Thread { get; set; }
        public int MsgLimit { get; set; } = 4;
        public bool LockMsg { get; set; } = false;  // 用于判断是否超出消息阀值
    }
}
