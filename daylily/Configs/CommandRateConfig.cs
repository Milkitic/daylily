using System.Collections.Concurrent;
using MilkiBotFramework.Plugining.Configuration;

// ReSharper disable once CheckNamespace
namespace daylily;

public sealed class CommandRateConfig : ConfigurationBase
{
    public ConcurrentDictionary<string, int> CommandRate { get; set; } = new();
}

public sealed class DragonLanguageConfig : ConfigurationBase
{
    public class UserExpression
    {
        public int Times { get; set; } = 1;
        public string Expression { get; set; }
    }

    public ConcurrentDictionary<string, List<UserExpression>>? UserDictionary { get; set; } = new();
}