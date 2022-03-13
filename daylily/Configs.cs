using System.Collections.Concurrent;
using System.ComponentModel;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining.Configuration;
using MilkiBotFramework.Utils;
using YamlDotNet.Serialization;

// ReSharper disable once CheckNamespace
namespace daylily;

public sealed class PluginManagerConfig : ConfigurationBase
{
    [Description("插件禁用列表")]
    [YamlMember(Alias = "DisabledList")]
    public ConcurrentDictionary<MessageIdentity, HashSet<Guid>> IdentityDisabledDictionary { get; set; } = new();
}

public sealed class TulingConfig : ConfigurationBase
{
    public ConcurrentDictionary<string, int>? ApiInfos { get; set; } = new();
}

public sealed class OsuConfig : ConfigurationBase
{
    public string QQAesKey { get; set; } = "aeskey";
    public string QQAesIV { get; set; } = "aesiv";
    public int ClientId { get; set; } = 114514;
    public string ClientSecret { get; set; } = "ClientSecret";
    public string ServerRedirectUri { get; set; } = "ServerRedirectUri";
    public int ServerRedirectPort { get; set; } = 23333;

}

public class KeywordTriggerConfig : ConfigurationBase
{
    public class TriggerObject
    {
        public TriggerObject()
        {
        }

        public TriggerObject(List<string> words, List<string> pictures, double chancePercent)
        {
            Words = words;
            Pictures = pictures;
            ChancePercent = chancePercent;
        }

        public List<string> Words { get; set; }
        public List<string> Pictures { get; set; }
        public double ChancePercent { get; set; }
    }

    public List<TriggerObject> UserDictionary { get; set; } = new List<TriggerObject>
    {
        new(new List<string> { "我" },
            new List<string>
            {
                "me1.jpg"
            }, 0.5),
        new(new List<string> { "你" },
            new List<string>
            {
                "you1.jpg", "you2.jpg"
            }, 2),
        new(new List<string> { "为啥", "为什么", "为毛", "为嘛", "why " },
            new List<string>
            {
                "why1.jpg"
            }, 20),
        new(new List<string> { "看来", "原来" },
            new List<string>
            {
                "kanlai1.jpg", "kanlai2.jpg"
            }, 30),
        new(new List<string> { "黄花菜" },
            new List<string>
            {
                "sb1.jpg", "sb2.jpg", "sb3.jpg", "sb4.jpg", "sb5.jpg", "sb6.jpg", "sb7.jpg", "sb8.jpg",
                "sb9.jpg"
            }, 50)
    };
}

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
public sealed class ShuntDownConfig : ConfigurationBase
{
    [YamlIgnore]
    internal readonly AsyncLock AsyncLock = new();
    public ConcurrentDictionary<MessageIdentity, DateTimeOffset?>? ExpireTimeDictionary { get; set; } = new();
}