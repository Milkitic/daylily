using System;
using Daylily.Bot.Message;

namespace Daylily.Bot.Session
{
    public class SessionReceivedEventArgs : EventArgs
    {
        public RouteMessage RouteMessageObj { get; set; }
    }
}