using System;

namespace Daylily.Bot
{
    public class ExceptionEventArgs : EventArgs
    {
        public ExceptionEventArgs(Exception e)
        {
            Exception = e;
        }
        public Exception Exception { get; }
    }
}