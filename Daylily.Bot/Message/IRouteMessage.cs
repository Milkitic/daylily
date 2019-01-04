using System;
using System.Collections.Generic;
using System.Text;
using Daylily.Bot.Session;

namespace Daylily.Bot.Message
{
    public interface IRouteMessage : IResponsiveMessage
    {
        IMessage Message { get; set; }
        ISessionIdentity Identity { get; }
        string UserId { get; }
    }
}
