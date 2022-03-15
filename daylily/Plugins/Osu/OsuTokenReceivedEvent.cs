using Coosu.Api.V2.ResponseModels;
using MilkiBotFramework.Event;

namespace daylily.Plugins.Osu;

public class OsuTokenReceivedEvent : IEventBusEvent
{
    public string FailReason { get; }
    public OsuTokenReceivedEvent(string failReason)
    {
        FailReason = failReason;
    }

    public OsuTokenReceivedEvent(string sourceId, long osuId, UserToken token)
    {
        SourceId = sourceId;
        OsuId = osuId;
        Token = token;
        IsSuccess = true;
    }

    public string? SourceId { get; }
    public long OsuId { get; }
    public UserToken? Token { get; }
    public bool IsSuccess { get; }
}