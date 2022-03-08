using System.Collections.Concurrent;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins.Core
{
    [PluginIdentifier("4c955ee3-3826-44a0-8c80-8f8507ead572")]
    [PluginLifetime(PluginLifetime.Singleton)]
    public class MessageLogging : BasicPlugin
    {
        private readonly ConcurrentDictionary<MessageIdentity, List<LightMessageContext>> _identityDictionary = new();

        public override IAsyncEnumerable<IResponse> OnMessageReceived(MessageContext context)
        {
            var lightMessageContext = new LightMessageContext
            {
                UserId = context.MessageUserIdentity?.UserId,
                RawMessage = context.TextMessage
            };
            _identityDictionary.AddOrUpdate(context.MessageIdentity!,
                new List<LightMessageContext>
                {
                    lightMessageContext
                }, (_, list) =>
                {
                    list.Add(lightMessageContext);
                    return list;
                }
            );

            return base.OnMessageReceived(context);
        }
    }

    internal class LightMessageContext
    {
        public string? UserId { get; set; }
        public string? RawMessage { get; set; }
    }
}
