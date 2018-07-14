using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Daylily.Common.Models.CQResponse;

namespace Daylily.Common.Models.MessageList
{
    public abstract class EndpointSettings
    {
        private readonly object _taskLock = new object();
        private Task _task;

        public abstract int MsgLimit { get; }

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
    }

    public abstract class EndpointSettings<T> : EndpointSettings where T : Msg
    {
        public ConcurrentQueue<T> MsgQueue { get; } = new ConcurrentQueue<T>();
        public abstract override int MsgLimit { get; }
    }
}
