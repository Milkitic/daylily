using System.Collections.Concurrent;
using System.ComponentModel;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining.Configuration;

// ReSharper disable once CheckNamespace
namespace daylily;

public sealed class PluginManagerConfig : ConfigurationBase
{
    [Description("插件禁用列表")]
    public ConcurrentDictionary<MessageIdentity, List<Guid>> IdentityDisabledDictionary { get; set; } = new();
}