using System;

namespace Daylily.Bot.Sessions.TreeStructure
{
    public class NavigatableNode : Node
    {
        public NavigatableNode(string name) : base(name)
        {
        }

        public NavigatableNode(string name, Func<object, ActionParam> actionCallback) : base(name)
        {
            Action = actionCallback;
        }

        public Func<object, ActionParam> Action { get; set; }
    }
}