using Daylily.Bot.Models;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Daylily.Bot
{
    public class Session : IDisposable
    {
        private static readonly ConcurrentDictionary<SessionId, Queue<CommonMessage>> Sessions =
            new ConcurrentDictionary<SessionId, Queue<CommonMessage>>();

        public int Timeout { get; set; }
        private readonly SessionId _sessionId;

        public Session(int timeout, Identity identity, params string[] userId) : this(timeout, identity,
            userId.Select(long.Parse).ToArray())
        { }

        public Session(int timeout, Identity identity, params long[] userId)
        {
            Timeout = timeout;
            _sessionId = new SessionId(identity, userId);

            if (!Sessions.Keys.Contains(_sessionId))
                Sessions.TryAdd(_sessionId, new Queue<CommonMessage>());
            else
                throw new NotSupportedException("尚不支持同时操作。");

            CoolQDispatcher.SessionReceived += Session_Received;
        }

        private void Session_Received(object sender, SessionReceivedEventArgs args)
        {
            if (_sessionId.Identity.Equals(args.MessageObj.Identity) &&
                _sessionId.UserId.Contains(long.Parse(args.MessageObj.UserId)))
                Sessions[_sessionId].Enqueue(args.MessageObj);
        }


        public async Task<CommonMessage> GetMessageAsync()
        {
            CancellationTokenSource cts = new CancellationTokenSource(Timeout);
            return await Task.Run(() =>
            {
                while (Sessions[_sessionId].Count < 1)
                {
                    if (cts.Token.IsCancellationRequested)
                        throw new TimeoutException($"Timed out. ({Timeout})");
                    Thread.Sleep(500);
                }

                return Sessions[_sessionId].Dequeue();
            }, cts.Token);
        }

        public void Dispose()
        {
            try
            {

            }
            finally
            {
                CoolQDispatcher.SessionReceived -= Session_Received;
                Sessions.TryRemove(_sessionId, out _);
            }
        }
    }

    public struct SessionId
    {
        public Identity Identity { get; }
        public long[] UserId { get; }

        public SessionId(Identity identity, long[] userId)
        {
            Identity = identity;
            UserId = userId;
        }
    }
}
