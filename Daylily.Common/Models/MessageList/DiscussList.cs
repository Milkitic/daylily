using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Daylily.Common.Assist;
using Daylily.Common.Interface.CQHttp;
using Daylily.Common.Models.CQResponse;
using Daylily.Common.Utils;
using Daylily.Common.Utils.LogUtils;

namespace Daylily.Common.Models.MessageList
{
    public class DiscussList
    {
        private readonly ConcurrentDictionary<long, DiscussSettings> _dicDiscuss = new ConcurrentDictionary<long, DiscussSettings>();

        public DiscussSettings this[long discussId] => _dicDiscuss[discussId];

        public void Add(long discussId)
        {
            if (_dicDiscuss.Keys.Contains(discussId))
                return;

            _dicDiscuss.GetOrAdd(discussId, new DiscussSettings(discussId.ToString()));
        }

    }

    public class DiscussSettings
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Queue<DiscussMsg> MsgQueue { get; set; } = new Queue<DiscussMsg>();
        public Task Task { get; set; }
        public int MsgLimit { get; set; } = 10;
        public bool LockMsg { get; set; } = false; // 用于判断是否超出消息阀值

        public DiscussSettings(string discussId)
        {
            Id = discussId;
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            if (Id == null) Logger.Warn("Id is null!!!!");
            try
            {
                var info = CqApi.GetGroupInfoV2(Id);
                string name = info == null ? Id : info.Data.GroupName;
                Name = name;
            }
            catch
            {
                Name = "组" + Id;
            }
        }
    }
}
