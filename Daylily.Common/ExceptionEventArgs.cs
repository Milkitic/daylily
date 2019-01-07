using System;

namespace Daylily.Common
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