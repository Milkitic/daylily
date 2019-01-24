using Daylily.Bot.Session;

namespace Daylily.Bot.Messaging
{
    public interface IRouteMessage : IResponsiveMessage
    {
        IMessage Message { get; set; }
        ISessionIdentity Identity { get; }
        string UserId { get; }
    }
}
