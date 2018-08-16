using System.Collections.Concurrent;
using Daylily.CoolQ.Models.CqResponse;

namespace Daylily.Bot.Models.MessageList
{
    public class PrivateList
    {
        public PrivateSettings this[long groupId] => _dicPri[groupId];

        private readonly ConcurrentDictionary<long, PrivateSettings> _dicPri = new ConcurrentDictionary<long, PrivateSettings>();

        public void Add(long privateId)
        {
            if (_dicPri.Keys.Contains(privateId))
                return;
            _dicPri.TryAdd(privateId, new PrivateSettings());
        }
    }

    public class PrivateSettings : EndpointSettings<PrivateMsg>
    {
        public override int MsgLimit { get; } = 4;
        public bool LockMsg { get; set; } = false; // 用于判断是否超出消息阀值
    }
}
