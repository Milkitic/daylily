using DaylilyWeb.Interface.CQHttp;
using DaylilyWeb.Models.CQResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DaylilyWeb.Assist
{
    public class DiscussList
    {
        Dictionary<long, DiscussSettings> dicDiscuss = new Dictionary<long, DiscussSettings>();

        public DiscussSettings this[long discussId]
        {
            get
            {
                return dicDiscuss[discussId];
            }
        }

        public void Add(long discussId)
        {
            if (dicDiscuss.Keys.Contains(discussId))
                return;

            dicDiscuss.Add(discussId, new DiscussSettings(discussId.ToString()));
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

        HttpApi CQApi = new HttpApi();

        public DiscussSettings(string discussId)
        {
            Id = discussId;
            UpdateInfo();
        }

        public void UpdateInfo()
        {
            var info = CQApi.GetGroupInfo(Id);
            string name = info == null ? Id : info.GroupName;
            Name = name;
        }
    }
}
