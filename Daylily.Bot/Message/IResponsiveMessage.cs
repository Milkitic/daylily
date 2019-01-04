using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Message
{
    public interface IResponsiveMessage
    {
        bool Handled { get; set; }
        object Tag { get; set; }
    }
}
