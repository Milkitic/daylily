using System;

namespace Daylily.Bot.Messaging
{
    public interface IResponsiveMessage
    {
        bool Handled { get; set; }
        bool Canceled { get; set; }
        bool IsForced { get; set; }
        TimeSpan DelayTime { get; set; }
        object Tag { get; set; }
    }
}
