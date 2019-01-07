using System;

namespace Daylily.CoolQ
{
    public class NoticeEventArgs : EventArgs
    {
        public NoticeEventArgs(object parsedObject)
        {
            ParsedObject = parsedObject;
        }
        public object ParsedObject { get; }
    }
}