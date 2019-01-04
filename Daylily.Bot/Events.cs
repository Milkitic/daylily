using Daylily.Bot.Message;
using Daylily.Bot.Models;
using System;

namespace Daylily.Bot
{
    public delegate void SessionReceivedEventHandler(object sender, SessionReceivedEventArgs args);

    public class SessionReceivedEventArgs : EventArgs
    {
        public INavigableMessage NavigableMessageObj { get; set; }
    }

    public delegate void MessageEventHandler(object sender, MessageEventArgs args);
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(object parsedObject)
        {
            ParsedObject = parsedObject;
        }

        public object ParsedObject { get; }
    }
    public delegate void NoticeEventHandler(object sender, NoticeEventArgs args);
    public class NoticeEventArgs : EventArgs
    {
        public NoticeEventArgs(object parsedObject)
        {
            ParsedObject = parsedObject;
        }
        public object ParsedObject { get; }
    }
    public delegate void RequestEventHandler(object sender, RequestEventArgs args);
    public class RequestEventArgs : EventArgs
    {
        public RequestEventArgs(object parsedObject)
        {
            ParsedObject = parsedObject;
        }

        public object ParsedObject { get; }
    }
}
