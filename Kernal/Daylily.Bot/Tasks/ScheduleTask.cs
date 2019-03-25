using System;
using System.Collections.Generic;
using Daylily.Bot.Session;

namespace Daylily.Bot.Tasks
{
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
}