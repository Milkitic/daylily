using System.Collections.Concurrent;
using MilkiBotFramework.Plugining.Configuration;

// ReSharper disable once CheckNamespace
namespace daylily;

public sealed class CommandRateConfig : ConfigurationBase
{
    public ConcurrentDictionary<string, int> CommandRate { get; set; } = new();
}