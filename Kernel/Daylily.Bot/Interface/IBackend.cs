using Daylily.Bot.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Interface
{
    public interface IBackend
    {
        BackendConfig BackendConfig { get; }
    }
}
