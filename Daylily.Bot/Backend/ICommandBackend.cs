using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Backend
{
    public interface ICommandBackend : IBackend
    {
        string[] Commands { get; set; }
    }
}
