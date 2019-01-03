using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Message
{
    public interface IResponse
    {
        bool Handled { get; set; }
        bool Cancel { get; set; }
        object Tag { get; set; }
    }
}
