using System;

namespace Daylily.Bot
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public object MessageObj { get; set; }
    }
}