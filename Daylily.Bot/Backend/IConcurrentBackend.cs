using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Backend
{
    public interface IConcurrentBackend
    {
        bool RunInMultiThreading { get; }
        bool RunInMultipleInstances { get; }
    }
}
