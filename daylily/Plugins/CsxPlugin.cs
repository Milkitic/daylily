using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using MilkiBotFramework.Connecting;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins;

[PluginIdentifier("77dabbaa-1f4a-49f0-9813-b6071936e7f6", "缩写查询")]
[Description("缩写查询 (https://github.com/itorr/nbnhhsh)")]
public class CsxPlugin : BasicPlugin
{
    private readonly LightHttpClient _lightHttpClient;

    public CsxPlugin(LightHttpClient lightHttpClient)
    {
        _lightHttpClient = lightHttpClient;
    }

    private static readonly Regex Regex = new("[A-za-z0-9]+");

    [CommandHandler("csx")]
    public async Task<IResponse> Guess(
        [Argument, Description("缩写词或段落")] string? text = null)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Reply("请提供关键词");

        var matches = Regex.Matches(text).Where(k => k.Success).ToArray();
        if (matches.Length == 0)
            return Reply("未检测到关键词");

        var enumerable = matches
            .SelectMany(k => k.Groups.Values
                .Where(o => o.Length > 1)
                .Select(o => o.Value))
            .Distinct()
            .ToList();

        if (enumerable.Count > 8)
        {
            return Reply("为防止刷屏，关键词不得同时超过8个");
        }

        text = string.Join(",", enumerable).ToLower();
        var result = await _lightHttpClient.HttpPost<Result[]>("https://lab.magiconch.com/api/nbnhhsh/guess",
            new Dictionary<string, object>
            {
                ["text"] = text
            }
        );

        if (result.Length == 0 || result.All(k => k.Trans == null) && result.All(k => k.Inputting == null))
            return Reply("未检测到结果");

        var ret = string.Join("\r\n", result
            .Where(k => k.Trans != null || k.Inputting != null)
            .Select(k =>
            {
                var sb = new StringBuilder(k.Name + ": ");
                if (k.Trans != null)
                {
                    sb.Append(string.Join(",", k.Trans));
                }
                else if (k.Inputting != null)
                {
                    sb.Append(string.Join(",", k.Inputting.Select(o => o + " (大概?)")));
                }

                return sb.ToString();
            }));
        return Reply(ret);
    }

    private class Result
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("trans")]
        public string[]? Trans { get; set; }

        [JsonPropertyName("inputting")]
        public string[]? Inputting { get; set; }
    }
}