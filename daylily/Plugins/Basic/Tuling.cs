using System.Collections.Concurrent;
using System.ComponentModel;
using daylily.ThirdParty.Tuling;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Messaging.RichMessages;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Plugining.Configuration;
using MilkiBotFramework.Tasking;

namespace daylily.Plugins.Basic;

[PluginIdentifier("14e60bec-dc11-45da-8654-baed42588745", "图灵")]
[Description("不时进行成精发言")]
[PluginLifetime(PluginLifetime.Singleton)]
public class Tuling : BasicPlugin
{
    private readonly BotTaskScheduler _taskScheduler;
    private readonly TulingClient _tulingClient;
    private readonly TulingConfig _config;

    public Tuling(IConfiguration<TulingConfig> configuration,
        BotTaskScheduler taskScheduler,
        TulingClient tulingClient)
    {
        _taskScheduler = taskScheduler;
        _tulingClient = tulingClient;
        _config = configuration.Instance;
    }

    protected override async Task OnInitialized()
    {
        _config.ApiInfos ??= new ConcurrentDictionary<string, int>();
        _taskScheduler.AddTask("ResetTuling", k => k
            .EachDayAt(new DateTime(1, 1, 1, 0, 0, 0, 0))
            .Do(ResetCount));
    }

    public override async IAsyncEnumerable<IResponse> OnMessageReceived(MessageContext context)
    {
        var rnd = Random.Shared.NextDouble();
        if (rnd >= 1d / 60)
        {
            yield break;
        }

        var text = await new RichMessage(context.GetRichMessage().Where(k => k is Text)).EncodeAsync();
        if (string.IsNullOrWhiteSpace(text))
        {
            yield break;
        }

        var apiInfos = _config.ApiInfos!;
        var apis = apiInfos
            .Where(k => k.Value < 100)
            .Select(k => k.Key)
            .ToArray();
        if (apis.Length == 0)
        {
            yield break;
        }

        var apiKey = apis[Random.Shared.Next(apis.Length)];

        var response = await _tulingClient.SendText(apiKey, text,
            context.MessageUserIdentity.UserId,
            context.MessageIdentity.ToString());
        if (response.Intent.Code == 4003)
        {
            apiInfos[apiKey] = 100;
        }
        else
        {
            apiInfos[apiKey]++;
        }

        await _config.SaveAsync();
        if (response.Intent.Code != 10004)
        {
            yield break;
        }

        var result = response.Results.FirstOrDefault();
        if (result?.Values.Text == null)
        {
            yield break;
        }

        var str = result.Values.Text.Trim('。', '？', '?', '！', '!', '～', '~');
        await Task.Delay(Random.Shared.Next(2, 7));
        yield return Reply(str, false);
    }

    private void ResetCount(TaskContext context, CancellationToken token)
    {
        foreach (var key in _config.ApiInfos.Keys)
        {
            _config.ApiInfos[key] = 0;
        }

        _config.SaveAsync().Wait(token);
    }
}