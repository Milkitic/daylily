using Daylily.Bot.Session;
using System;

namespace Daylily.Bot.Tasks
{
    public class IntervalTask : ApplicationTask
    {
        public IntervalTask(ISessionIdentity identity, Action callback, TimeSpan interval) : base(identity, callback, TaskType.IntervalTask)
        {
            Interval = interval;
        }

        public TimeSpan Interval { get; }
    }
}