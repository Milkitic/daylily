using Daylily.Bot.Message;
using Daylily.Bot.Session;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Dispatcher
{
    public abstract class CompoundDispatcher : IMessageDispatcher, IEventDispatcher, ISessionDispatcher
    {
        public event ExceptionEventHandler ErrorOccured;
        public event SessionReceivedEventHandler SessionReceived;
        public abstract MiddlewareConfig MiddlewareConfig { get; }
        public abstract bool Message_Received(object sender, MessageEventArgs args);
        public abstract void SendMessage(RouteMessage message);
        public abstract bool Event_Received(object sender, EventEventArgs args);

        protected virtual bool RaiseSessionEvent(RouteMessage message)
        {
            if (SessionReceived != null)
            {
                SessionReceived?.Invoke(null, new SessionReceivedEventArgs
                {
                    RouteMessageObj = message
                });
                return true;
            }

            return false;
        }
    }
}
