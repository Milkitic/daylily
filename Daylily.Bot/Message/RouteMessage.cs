using Daylily.Bot.Command;
using System;
using System.Collections.Generic;
using System.Text;
using Daylily.Bot.Session;

namespace Daylily.Bot.Message
{
    public abstract class RouteMessage : ICommand, IRouteMessage
    {
        public bool Handled { get; set; } = false;
        public object Tag { get; set; }
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

        public string RawMessage => Message.RawMessage;
    }
}
