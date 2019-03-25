using Daylily.Bot.Session;
using System;

namespace Daylily.Bot.Tasks
{
    public abstract class ApplicationTask
    {
        protected ApplicationTask(ISessionIdentity identity, Action callback, TaskType taskType) : this(identity, callback, TaskPriority.Normal, taskType)
        {
            Identity = identity;
            Callback = callback;
        }

        protected ApplicationTask(ISessionIdentity identity, Action callback, TaskPriority taskPriority, TaskType taskType)
        {
            Identity = identity;
            Callback = callback;
            TaskPriority = taskPriority;
            TaskType = taskType;
        }

        public ISessionIdentity Identity { get; }
        public Action Callback { get; }
        public TaskPriority TaskPriority { get; }
        public TaskType TaskType { get; }
    }

    public enum TaskType
    {
        ScheduleTask, IntervalTask
    }
}