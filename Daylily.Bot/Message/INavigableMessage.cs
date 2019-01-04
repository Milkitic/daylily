using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Message
{
    public interface INavigableMessage
    {
        IMessage Message { get; set; }
        ISessionIdentity Identity { get; }
        string UserId { get; }
    }
}
