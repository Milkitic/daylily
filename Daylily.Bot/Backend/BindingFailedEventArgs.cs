using System;
using System.Collections.Generic;

namespace Daylily.Bot.Backend
{
    public class BindingFailedEventArgs : EventArgs
    {
        public BindingFailedEventArgs(List<BindingFailedItem> failedItems)
        {
            FailedItems = failedItems;
        }
        public List<BindingFailedItem> FailedItems { get; }

        public struct BindingFailedItem
        {
            public string Parameter { get; set; }
            public string Value { get; set; }
            public string Reason { get; set; }
        }
    }
}