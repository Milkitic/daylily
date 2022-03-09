using System.Collections.Concurrent;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins.Core;

[PluginIdentifier("4c955ee3-3826-44a0-8c80-8f8507ead572", AllowDisable = false)]
[PluginLifetime(PluginLifetime.Singleton)]
public class MessageLogging : BasicPlugin
{
    public ConcurrentDictionary<MessageIdentity, List<LightMessage>> IdentityDictionary { get; } = new();

    public override IAsyncEnumerable<IResponse> OnMessageReceived(MessageContext context)
    {
        var lightMessageContext = new LightMessage
        {
            UserId = context.MessageUserIdentity?.UserId,
            RawMessage = context.TextMessage,
            Timestamp = context.ReceivedTime
        };
        IdentityDictionary.AddOrUpdate(context.MessageIdentity!,
            new List<LightMessage>
            {
                lightMessageContext
            }, (_, list) =>
            {
                if (list.Count > 20)
                    list.RemoveAt(0);
                list.Add(lightMessageContext);
                return list;
            }
        );

        return base.OnMessageReceived(context);
    }
}

public class LightMessage
{
    public string? UserId { get; set; }
    public string? RawMessage { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}