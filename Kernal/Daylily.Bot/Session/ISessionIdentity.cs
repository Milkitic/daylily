namespace Daylily.Bot.Session
{
    public interface ISessionIdentity
    {
        string Identity { get; set; }
        string SessionType { get; set; }
    }
}
