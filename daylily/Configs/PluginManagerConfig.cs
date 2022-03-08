using System.Collections.Concurrent;
using System.ComponentModel;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining.Configuration;
using YamlDotNet.Serialization;

// ReSharper disable once CheckNamespace
namespace daylily;

public sealed class PluginManagerConfig : ConfigurationBase
{
    [Description("插件禁用列表")]
    [YamlMember(Alias = "DisabledList")]
    public ConcurrentDictionary<MessageIdentity, HashSet<Guid>> IdentityDisabledDictionary { get; set; } = new();
}