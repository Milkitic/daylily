using System;

namespace Daylily.Bot.Message
{
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(object parsedObject)
        {
            ParsedObject = parsedObject;
        }

        public object ParsedObject { get; }
    }
}