using Daylily.Bot.Message;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Daylily.Bot.Session
{
    public class Session : IDisposable
    {
        private static readonly ConcurrentDictionary<SessionId, Queue<RouteMessage>> Sessions =
            new ConcurrentDictionary<SessionId, Queue<RouteMessage>>();

        private static readonly object LockObj = new object(); // static: for safe

        public int Timeout { private get; set; }

        public SessionId SessionId => _sessionId;
        private SessionId _sessionId;

        public Session(int timeout, ISessionIdentity cqIdentity, params string[] userId) : this(timeout, cqIdentity,
            userId.Select(long.Parse).ToArray())
        { }

        public Session(int timeout, ISessionIdentity cqIdentity, params long[] userId)
        {
            Timeout = timeout;
            _sessionId = new SessionId(cqIdentity, userId);

            if (Sessions.Keys.Any(item => item.Contains(SessionId)))
                throw new NotSupportedException("不支持同时两个会话操作。");

            Sessions.TryAdd(SessionId, new Queue<RouteMessage>());
            if (DaylilyCore.Current.SessionDispatcher != null)
            {
                DaylilyCore.Current.SessionDispatcher.SessionReceived += Session_Received;
            }
        }

        public void AddMember(params long[] userId)
        {
            lock (LockObj)
            {
                Sessions.TryRemove(SessionId, out var msgQueue);
                _sessionId.UserId = SessionId.UserId.Concat(userId).ToArray();
                Sessions.TryAdd(SessionId, msgQueue);
            }
        }

        public void AddMember(params string[] userId)
        {
            AddMember(userId.Select(long.Parse).ToArray());
        }

        public RouteMessage GetMessage()
        {
            TryGetMessage(out var messageObj);
            return messageObj ?? throw new TimeoutException($"Timed out. ({Timeout})");
        }

        public bool TryGetMessage(out RouteMessage routeMessageObj)
        {
            CancellationTokenSource cts = new CancellationTokenSource(Timeout);
            routeMessageObj = Task.Run(() =>
            {
                int count;
                lock (LockObj)
                    count = Sessions[SessionId].Count;

                while (count < 1)
                {
                    if (cts.Token.IsCancellationRequested)
                        return null;
                    Thread.Sleep(500);
                    lock (LockObj)
                        count = Sessions[SessionId].Count;
                }

                lock (LockObj)
                    return Sessions[SessionId].Dequeue();
            }, cts.Token).Result;

            return routeMessageObj != null;
        }

        public void Dispose()
        {
            try
            {

            }
            finally
            {
                if (DaylilyCore.Current.SessionDispatcher != null)
                {
                    DaylilyCore.Current.SessionDispatcher.SessionReceived -= Session_Received;
                }
                Sessions.TryRemove(SessionId, out _);
            }
        }

        private void Session_Received(object sender, SessionReceivedEventArgs args)
        {
            lock (LockObj)
                if (SessionId.CqIdentity == args.RouteMessageObj.Identity &&
                    SessionId.UserId.Contains(long.Parse(args.RouteMessageObj.UserId)))
                    Sessions[SessionId].Enqueue(args.RouteMessageObj);
        }
    }

    public struct SessionId
    {
        public ISessionIdentity CqIdentity { get; }

        public long[] UserId { get; set; }

        public SessionId(ISessionIdentity cqIdentity, long[] userId)
        {
            CqIdentity = cqIdentity;
            UserId = userId;
        }

        /// <summary>
        /// 单用户会话
        /// </summary>
        public bool Equals(SessionId obj) => CqIdentity == obj.CqIdentity && UserId.SequenceEqual(obj.UserId);

        /// <summary>
        /// 要求更为严格的会话
        /// </summary>
        public bool Contains(SessionId obj)
        {
            long[] userId = UserId;
            return CqIdentity.Equals(obj.CqIdentity) && obj.UserId.Any(k => userId.Contains(k));
        }

        public override bool Equals(object obj) => !(obj is null) && obj is SessionId id && Equals(id);

        public override int GetHashCode() => HashCode.Combine(CqIdentity, UserId);

        public static bool operator !=(SessionId s1, SessionId s2) => !s1.Equals(s2);

        public static bool operator ==(SessionId s1, SessionId s2) => s1.Equals(s2);
    }
}
