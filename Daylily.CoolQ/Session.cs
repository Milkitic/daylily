using Daylily.Bot.Message;
using Daylily.CoolQ.CoolQHttp;
using Daylily.CoolQ.CoolQHttp.ResponseModel.Abstract;
using Daylily.CoolQ.CoolQHttp.ResponseModel.Report;
using Daylily.CoolQ.Message;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daylily.CoolQ
{
    public class Session
    {
        public CoolQIdentity Identity { get; }
        public int MsgLimit { get; }
        public bool LockMsg { get; set; } = false; // 用于判断是否超出消息阀值

        public ConcurrentQueue<object> MsgQueue { get; } = new ConcurrentQueue<object>();
        private SessionData _data;
        private DateTime _prevUpdateTime;

        public Session(CoolQIdentity identity)
        {
            Identity = identity;
            switch (identity.Type)
            {
                case MessageType.Private:
                    MsgLimit = 4;
                    break;
                case MessageType.Discuss:
                    MsgLimit = 10;
                    break;
                case MessageType.Group:
                    MsgLimit = 10;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<SessionData> GetDataAsync()
        {
            if (_data == null || DateTime.Now - _prevUpdateTime > TimeSpan.FromDays(1))
            {
                await UpdateDataAsync();
                if (_data == null)
                    throw new Exception("获取会话数据失败，请检查 HTTP API 或 酷Q 是否运行正常。");
            }

            return _data;
        }

        private async Task UpdateDataAsync()
        {
            await Task.Run(() => { UpdateData(); });
        }

        public void UpdateData()
        {
            _prevUpdateTime = DateTime.Now;
            switch (Identity.Type)
            {
                case MessageType.Private:
                    _data = new SessionData(UpdatePrivateInfo(Identity.Id));
                    break;
                case MessageType.Discuss:
                    _data = new SessionData($"组{Identity.Id}");
                    break;
                case MessageType.Group:
                    _data = new SessionData(UpdateGroupInfo(Identity.Id));
                    break;
            }
        }

        private readonly object _taskLock = new object();
        private Task _task;

        public bool TryRun(Action action)
        {
            bool isTaskFree = _task == null || _task.IsCanceled || _task.IsCompleted;
            if (isTaskFree)
            {
                lock (_taskLock)
                {
                    isTaskFree = _task == null || _task.IsCanceled || _task.IsCompleted;
                    if (isTaskFree)
                    {
                        _task = Task.Run(action);
                    }
                }
            }

            return isTaskFree;
        }

        private static StrangerInfo UpdatePrivateInfo(long id)
        {
            StrangerInfo obj = null;
            try
            {
                obj = CoolQHttpApiClient.GetStrangerInfo(id.ToString()).Data;
            }
            catch
            {
                //obj = InitInfo();
            }

            return obj/* ?? InitInfo()*/;

            //StrangerInfo InitInfo()
            //{
            //    var groupInfoV2 = new StrangerInfo
            //    {
            //        Nickname = new CoolQIdentity(id, MessageType.Private).ToString(),
            //        UserId = id,
            //        Age = "-1",
            //        Sex = "unknown",
            //    };
            //    return groupInfoV2;
            //}
        }

        private static GroupInfoV2 UpdateGroupInfo(long id)
        {
            GroupInfoV2 obj = null;
            try
            {
                obj = CoolQHttpApiClient.GetGroupInfoV2(id.ToString()).Data;
            }
            catch
            {
                //obj = InitInfo();
            }

            return obj/* ?? InitInfo()*/;

            //GroupInfoV2 InitInfo()
            //{
            //    var groupInfoV2 = new GroupInfoV2
            //    {
            //        GroupName = new CoolQIdentity(id, MessageType.Group).ToString(),
            //        GroupId = long.Parse(id.ToString()),
            //        Admins = new List<GroupInfoV2Admins>(),
            //    };
            //    return groupInfoV2;
            //}
        }
    }

    public class SessionData
    {
        public SessionData(string name) : this(name, null, null)
        {
        }

        public SessionData(StrangerInfo privateInfo) : this(null, null, privateInfo)
        {
            Name = privateInfo.Nickname;
        }
        public SessionData(GroupInfoV2 groupInfo) : this(null, groupInfo, null)
        {
            Name = groupInfo.GroupName;
        }

        public SessionData(string name, StrangerInfo privateInfo) : this(name, null, privateInfo)
        {
        }

        public SessionData(string name, GroupInfoV2 groupInfo) : this(name, groupInfo, null)
        {
        }

        private SessionData(string name, GroupInfoV2 groupInfo, StrangerInfo privateInfo)
        {
            Name = name;
            PrivateInfo = privateInfo;
            GroupInfo = groupInfo;
        }

        public string Name { get; private set; }
        public GroupInfoV2 GroupInfo { get; private set; }
        public StrangerInfo PrivateInfo { get; private set; }
    }
}
