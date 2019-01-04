using System;

namespace Daylily.CoolQ
{
    public class RequestEventArgs : EventArgs
    {
        public RequestEventArgs(object parsedObject)
        {
            ParsedObject = parsedObject;
        }

        public object ParsedObject { get; }
    }
}
