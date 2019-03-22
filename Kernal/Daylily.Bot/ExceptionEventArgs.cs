using System;

namespace Daylily.Bot
{
    public class ExceptionEventArgs : EventArgs
    {
        public ScopeEventArgs Scope { get; }

        public ExceptionEventArgs(ScopeEventArgs scope, Exception e) : this(scope, e, null)
        {

        }

        public ExceptionEventArgs(ScopeEventArgs scope, Exception e, string causedMessage)
        {
            Scope = scope;
            CausedMessage = causedMessage;
            Exception = e;
        }

        public string CausedMessage { get; }

        public Exception Exception { get; }
    }
}