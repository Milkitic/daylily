using System.Collections.Concurrent;
using daylily.Plugins.Core;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Plugining.Loading;

namespace daylily.Plugins.Services
{
    [PluginIdentifier("365d4d8b-e5f3-4815-9503-a9416fc28af5", "发言重复过滤")]
    public class MessageAvoidRepeatService : ServicePlugin
    {
        private readonly ConcurrentDictionary<MessageIdentity, List<LightMessage>> _identityDictionary = new();

        public override async Task<bool> BeforeSend(PluginInfo pluginInfo, IResponse response)
        {
            var id = response.MessageContext?.MessageIdentity;
            if (id == null) return true;

            if (response.Message == null) return true;
            var message = await response.Message.EncodeAsync();

            var list = _identityDictionary.GetOrAdd(id, _ => new List<LightMessage>());

            var dueMessage = list.FirstOrDefault(k => k.Timestamp.AddSeconds(30) < DateTimeOffset.Now);
            if (dueMessage != null)
            {
                var index = list.IndexOf(dueMessage);
                list.RemoveRange(0, index + 1);
            }

            if (list.Any(k => k.RawMessage == message))
            {
                if (response.IsForced == true) return true;

                response.Handled();
                return false;
            }

            var lightMessage = new LightMessage { RawMessage = message, Timestamp = DateTimeOffset.Now };
            list.Add(lightMessage);
            return true;
        }
    }
}
