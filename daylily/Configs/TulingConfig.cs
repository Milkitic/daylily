using System.Collections.Concurrent;
using MilkiBotFramework.Plugining.Configuration;

namespace daylily;

public sealed class TulingConfig : ConfigurationBase
{
    public ConcurrentDictionary<string, int>? ApiInfos { get; set; } = new();
}