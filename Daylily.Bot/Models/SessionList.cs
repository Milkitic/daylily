using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Daylily.Bot.Enum;
using Daylily.Bot.Models.MessageList;
using Daylily.CoolQ.Interface.CqHttp;
using Daylily.CoolQ.Models.CqResponse;
using Daylily.CoolQ.Models.CqResponse.Api;

namespace Daylily.Bot.Models
{
    public class SessionList
    {
        private readonly ConcurrentDictionary<(long id, MessageType type), SessionSettings> _dicSession =
            new ConcurrentDictionary<(long id, MessageType type), SessionSettings>();

        public SessionSettings this[(long id, MessageType type) identity] => _dicSession[identity];

        public void Add(Msg message)
        {
            switch (message)
            {
                case PrivateMsg privateMsg:
                    {
                        var item = (privateMsg.UserId, MessageType.Private);
                        if (_dicSession.Keys.Contains(item))
                            return;
                        _dicSession.TryAdd(item, new SessionSettings(privateMsg));
                        break;
                    }
                case DiscussMsg discussMsg:
                    {
                        var item = (discussMsg.DiscussId, MessageType.Discuss);
                        if (_dicSession.Keys.Contains(item))
                            return;
                        _dicSession.TryAdd(item, new SessionSettings(discussMsg));
                        break;
                    }
                case GroupMsg groupMsg:
                    {
                        var item = (groupMsg.GroupId, MessageType.Group);
                        if (_dicSession.Keys.Contains(item))
                            return;
                        _dicSession.TryAdd(item, new SessionSettings(groupMsg));
                        break;
                    }
            }
        }

        public class SessionSettings
        {
            public string Id { get; }
            public string Name { get; }
            public MessageType MessageType { get; }
            public int MsgLimit { get; }
            public bool LockMsg { get; set; } = false; // 用于判断是否超出消息阀值
            public GroupInfoV2 Info { get; }
            public ConcurrentQueue<object> MsgQueue { get; } = new ConcurrentQueue<object>();

            public SessionSettings(Msg message)
            {
                switch (message)
                {
                    case PrivateMsg privateMsg:
                        Id = privateMsg.UserId.ToString();
                        MessageType = MessageType.Private;
                        MsgLimit = 4;
                        Name = "私聊" + Id; // todo
                        break;
                    case DiscussMsg discussMsg:
                        Id = discussMsg.DiscussId.ToString();
                        MessageType = MessageType.Discuss;
                        MsgLimit = 10;
                        Name = "组" + Id; // todo
                        break;
                    case GroupMsg groupMsg:
                        Id = groupMsg.GroupId.ToString();
                        MessageType = MessageType.Group;
                        MsgLimit = 10;
                        Info = UpdateGroupInfo(groupMsg.GroupId);
                        Name = Info.GroupName;
                        break;
                }
            }

            private static GroupInfoV2 UpdateGroupInfo(long id)
            {
                GroupInfoV2 obj;
                try
                {
                    obj = CqApi.GetGroupInfoV2(id.ToString()).Data;
                }
                catch
                {
                    obj = InitInfo();
                }

                return obj ?? InitInfo();

                GroupInfoV2 InitInfo()
                {
                    var groupInfoV2 = new GroupInfoV2
                    {
                        GroupName = "群" + id,
                        GroupId = long.Parse(id.ToString()),
                        Admins = new List<GroupInfoV2Admins>(),
                    };
                    return groupInfoV2;
                }
            }
        }
    }
}
