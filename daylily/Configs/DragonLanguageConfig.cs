using System.Collections.Concurrent;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining.Configuration;
using MilkiBotFramework.Utils;
using YamlDotNet.Serialization;

namespace daylily;

public sealed class DragonLanguageConfig : ConfigurationBase
{
    public class UserExpression
    {
        public int Times { get; set; } = 1;
        public string Expression { get; set; }
    }

    public ConcurrentDictionary<string, List<UserExpression>>? UserDictionary { get; set; } = new();
}
public sealed class ShuntDownConfig : ConfigurationBase
{
    [YamlIgnore]
    internal readonly AsyncLock AsyncLock = new();
    public ConcurrentDictionary<MessageIdentity, DateTimeOffset?>? ExpireTimeDictionary { get; set; } = new();
}