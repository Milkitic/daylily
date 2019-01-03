using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Interface
{
    public interface ISessionIdentity
    {
        string Identity { get; set; }
        string SessionType { get; set; }
    }
}
