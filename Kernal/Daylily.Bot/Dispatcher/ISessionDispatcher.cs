using System;
using System.Collections.Generic;
using System.Text;
using Daylily.Bot.Session;

namespace Daylily.Bot.Dispatcher
{
    public interface ISessionDispatcher : IDispatcher
    {
        event SessionReceivedEventHandler SessionReceived;
    }
}
