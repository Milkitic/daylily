using System;
using System.Collections.Generic;
using System.Text;
using Daylily.Bot.Session;

namespace Daylily.Bot.Tasks
{
    public class TaskManager
    {
        public void AddTask(ApplicationTask task)
        {

        }
    }

    public abstract class ApplicationTask
    {
        protected ApplicationTask(ISessionIdentity identity, Action callback)
        {
            Identity = identity;
            Callback = callback;
        }

        public ISessionIdentity Identity { get; set; }
        public Action Callback { get; set; }
    }

    public class ScheduleTask : ApplicationTask
    {
        public ScheduleTask(ISessionIdentity identity, Action callback, params DateTime[] triggerTime)
            : base(identity, callback)
        {
            _triggerTimes = new HashSet<DateTime>(triggerTime);
        }

        private readonly HashSet<DateTime> _triggerTimes;

        public IReadOnlyCollection<DateTime> TriggerTimes => _triggerTimes;
    }

    public class IntervalTask : ApplicationTask
    {
        public IntervalTask(ISessionIdentity identity, Action callback, TimeSpan interval) : base(identity, callback)
        {
            Interval = interval;
        }

        public TimeSpan Interval { get; }
    }
}
