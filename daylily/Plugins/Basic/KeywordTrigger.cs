using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Messaging.RichMessages;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Plugining.Configuration;

namespace daylily.Plugins.Basic;

[PluginIdentifier("1abf7c43-bafb-4536-a2be-e9aff8a2fc51", "关键词触发")]
[Description("收到已给的关键词时，根据已给几率返回一张熊猫图。")]
public class KeywordTrigger : BasicPlugin
{
    private readonly KeywordTriggerConfig _config;

    public KeywordTrigger(IConfiguration<KeywordTriggerConfig> configuration)
    {
        _config = configuration.Instance;
    }

    protected override async Task OnInitialized()
    {
        _config.UserDictionary.Sort(new TriggerComparer());
        _config.UserDictionary.RemoveAll(p => p == null);
        await _config.SaveAsync();
    }

    public override async IAsyncEnumerable<IResponse> OnMessageReceived(MessageContext context)
    {
        var msg = await new RichMessage(context.GetRichMessage().Where(k => k is Text))
            .EncodeAsync();
        foreach (var item in _config.UserDictionary)
        {
            if (Trig(msg, item.Words, item.Pictures, out var img, item.ChancePercent))
            {
                yield return Reply(new FileImage(Path.Combine(PluginHome, "panda", img)));
                yield break;
            }
        }
    }

    private static bool Trig(string message, IEnumerable<string> keywords, IReadOnlyList<string> pics,
        [NotNullWhen(true)] out string? imgP, double chancePercent = 10)
    {
        var chance = chancePercent / 100d;
        if (keywords.Any(k => message.Contains(k, StringComparison.InvariantCultureIgnoreCase)))
        {
            imgP = pics[Random.Shared.Next(pics.Count)];
            return Random.Shared.NextDouble() < chance;
        }

        imgP = null;
        return false;
    }

    private class TriggerComparer : IComparer<KeywordTriggerConfig.TriggerObject>
    {
        public int Compare(KeywordTriggerConfig.TriggerObject? x, KeywordTriggerConfig.TriggerObject? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            return x.ChancePercent.CompareTo(y.ChancePercent);
        }
    }
}