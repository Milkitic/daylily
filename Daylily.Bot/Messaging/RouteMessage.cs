using Daylily.Bot.Backend;
using Daylily.Bot.Command;
using Daylily.Bot.Session;
using System;
using System.Collections.Generic;

namespace Daylily.Bot.Messaging
{
    public abstract class RouteMessage : IWritableCommand, IRouteMessage
    {
        #region IResponsiveMessage
        public bool Handled { get; set; } = false;
        public bool Canceled { get; set; } = false;
        public bool IsForced { get; set; } = false;
        public TimeSpan DelayTime { get; set; } = TimeSpan.FromSeconds(0);
        public object Tag { get; set; }
        #endregion

        #region IWritableCommand
        public string FullCommand { get; set; }
        public string CommandName { get; set; }
        public string ArgString { get; set; }
        public Dictionary<string, string> Args { get; set; }
        public List<string> FreeArgs { get; set; }
        public List<string> Switches { get; set; }
        public List<string> SimpleArgs { get; set; }

        #endregion

        #region IRouteMessage
        public virtual IMessage Message { get; set; }
        public virtual ISessionIdentity Identity { get; }
        public string UserId { get; set; }
        #endregion

        public string RawMessage => Message.RawMessage;
    }
}
