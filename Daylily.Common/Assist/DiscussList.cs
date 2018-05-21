using Daylily.Common.Interface.CQHttp;
using Daylily.Common.Models.CQResponse;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Daylily.Common.Assist
{
    public class DiscussList
    {
        private readonly Dictionary<long, DiscussSettings> _dicDiscuss = new Dictionary<long, DiscussSettings>();

        public DiscussSettings this[long discussId] => _dicDiscuss[discussId];

        public void Add(long discussId)
        {
            if (_dicDiscuss.Keys.Contains(discussId))
                return;

            _dicDiscuss.Add(discussId, new DiscussSettings(discussId.ToString()));
        }

    }

    public class DiscussSettings
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Queue<DiscussMsg> MsgQueue { get; set; } = new Queue<DiscussMsg>();
        public Thread Thread { get; set; }
        public int MsgLimit { get; set; } = 10;
        public bool LockMsg { get; set; } = false;  // 用于判断是否超出消息阀值

        private readonly HttpApi _cqApi = new HttpApi();

        public DiscussSettings(string discussId)
        {
            Id = discussId;
            UpdateInfo();
        }

        public void UpdateInfo()
        {
            var info = _cqApi.GetGroupInfo(Id);
            string name = info == null ? Id : info.GroupName;
            Name = name;
        }
    }
}
