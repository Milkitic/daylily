using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Message
{
    public interface IRouteMessage
    {
        IMessage Message { get; set; }
        ISessionIdentity Identity { get; }
        string UserId { get; }
    }
}
