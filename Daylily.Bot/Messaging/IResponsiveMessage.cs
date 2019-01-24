namespace Daylily.Bot.Messaging
{
    public interface IResponsiveMessage
    {
        bool Handled { get; set; }
        bool Canceled { get; set; }
        object Tag { get; set; }
    }
}
