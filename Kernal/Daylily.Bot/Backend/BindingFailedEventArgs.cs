using System;
using System.Collections.Generic;

namespace Daylily.Bot.Backend
{
    public class BindingFailedEventArgs : EventArgs
    {
        public BindingFailedEventArgs(ScopeEventArgs scope, BindingFailedItem failedItem)
        {
            Scope = scope;
            FailedItem = failedItem;
        }

        public ScopeEventArgs Scope { get; }
        public BindingFailedItem FailedItem { get; }
    }

    public class BindingFailedItem
    {
        public string Parameter { get; set; }
        public string Value { get; set; }
        public string Reason { get; set; }
    }
}