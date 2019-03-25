using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daylily.Bot.Tasks
{
    public class TaskManager : IDisposable
    {
        public static TaskManager Instance { get; private set; }

        private readonly ConcurrentDictionary<string, HashSet<ApplicationTask>> _taskLists =
            new ConcurrentDictionary<string, HashSet<ApplicationTask>>();

        private HashSet<Task> StartingTask;

        public TaskManager()
        {
            if (Instance == null)
                Instance = this;
            else
                return;

            RunInternal();
        }

        public void AddOrKeepTask(string typeName, ApplicationTask task)
        {
            switch (task.TaskPriority)
            {
                case TaskPriority.Normal:
                    if (_taskLists.ContainsKey(typeName))
                    {
                        if (task is IntervalTask intervalTask)
                        {
                            var existTask = _taskLists[typeName]
                                .FirstOrDefault(k => k.TaskType == TaskType.IntervalTask);
                            switch (existTask)
                            {
                                case IntervalTask exist:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        create = true;
                    }
                    break;
                case TaskPriority.High:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RunInternal()
        {
            foreach (var pair in _taskLists)
            {

            }

        }

        public void Dispose()
        {
        }
    }
}
