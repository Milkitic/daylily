using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Daylily.Common.Models.CQResponse;

namespace Daylily.Common.Models.MessageList
{
    public class PrivateList
    {
        public PrivateSettings this[long groupId] => _di[groupId];

        private readonly ConcurrentDictionary<long, PrivateSettings> _di = new ConcurrentDictionary<long, PrivateSettings>();

        public void Add(long privateId) => _di.TryAdd(privateId, new PrivateSettings());
    }

    public class PrivateSettings : EndpointSettings<PrivateMsg>
    {
        public override int MsgLimit { get; } = 4;
        public bool LockMsg { get; set; } = false; // 用于判断是否超出消息阀值
    }
}
