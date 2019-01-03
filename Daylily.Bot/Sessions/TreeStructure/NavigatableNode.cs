using System;

namespace Daylily.Bot.Sessions.TreeStructure
{
    public class NavigatableNode : Node
    {
        public NavigatableNode(string name) : base(name)
        {
        }

        public NavigatableNode(string name, Func<object, Action> actionCallback) : base(name)
        {
            Action = actionCallback;
        }

        public Func<object, Action> Action { get; set; }
    }
}