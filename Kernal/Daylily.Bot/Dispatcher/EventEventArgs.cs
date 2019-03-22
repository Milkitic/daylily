using System;

namespace Daylily.Bot.Dispatcher
{
    public class EventEventArgs : EventArgs
    {
        public EventEventArgs(object parsedObject)
        {
            ParsedObject = parsedObject;
        }

        public object ParsedObject { get; }
    }
}