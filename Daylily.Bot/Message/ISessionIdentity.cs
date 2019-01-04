namespace Daylily.Bot.Message
{
    public interface ISessionIdentity
    {
        string Identity { get; set; }
        string SessionType { get; set; }
    }
}
