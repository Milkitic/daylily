using Coosu.Api.V2.ResponseModels;
using MilkiBotFramework.Event;

namespace daylily.Plugins.Osu;

public class OsuTokenReceivedEvent : IEventBusEvent
{
    public OsuTokenReceivedEvent(string sessionToken, string failReason)
    {
        SessionToken = sessionToken;
        FailReason = failReason;
    }

    public OsuTokenReceivedEvent(string sessionToken, string sourceId, User user, UserToken token)
    {
        SessionToken = sessionToken;
        IsSuccess = true;
        SourceId = sourceId;
        User = user;
        Token = token;
    }

    public string SessionToken { get; }
    public bool IsSuccess { get; }
    public string? FailReason { get; }
    public string? SourceId { get; }
    public User? User { get; }
    public UserToken? Token { get; }
}