using System;
using Daylily.Bot.Messaging;

namespace Daylily.Bot.Session
{
    public class SessionReceivedEventArgs : EventArgs
    {
        public RouteMessage RouteMessageObj { get; set; }
    }
}