using Daylily.Bot.Command;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Message
{
    public abstract class NavigableMessage : ICommand, INavigableMessage
    {
        public string FullCommand { get; set; }
        public string Command { get; set; }
        public string ArgString { get; set; }
        public Dictionary<string, string> Args { get; set; }
        public List<string> FreeArgs { get; set; }
        public Dictionary<string, string> Switches { get; set; }
        public List<string> SimpleArgs { get; set; }
        public virtual IMessage Message { get; set; }
        public virtual ISessionIdentity Identity { get; }
        public string UserId { get; set; }
    }
}
