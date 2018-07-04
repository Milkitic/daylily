using System.Collections.Concurrent;
using Daylily.Common.Models.CQResponse;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Daylily.Common.Assist
{
    public class PrivateList
    {
        public PrivateSettings this[long groupId] => _di[groupId];

        private readonly ConcurrentDictionary<long, PrivateSettings> _di = new ConcurrentDictionary<long, PrivateSettings>();

        public void Add(long privateId)
        {
            if (_di.Keys.Contains(privateId))
                return;
            _di.GetOrAdd(privateId, new PrivateSettings());
        }
    }

    public class PrivateSettings
    {
        public Queue<PrivateMsg> MsgQueue { get; set; } = new Queue<PrivateMsg>();
        public Thread Thread { get; set; }
        public int MsgLimit { get; set; } = 4;
        public bool LockMsg { get; set; } = false; // 用于判断是否超出消息阀值
    }
}
