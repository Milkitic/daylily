﻿using System.Collections.Concurrent;
using System.ComponentModel;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Plugining.Configuration;

namespace daylily.Plugins;

[PluginIdentifier("fe577f01-b63f-45e2-88bd-3236224b93b9", "命令统计", Index = -10)]
[Description("统计命令的使用情况，分析常用命令")]
public class CommandRateService : BasicPlugin
{
    private readonly IConfiguration<CommandRateConfig> _config;

    public CommandRateService(IConfiguration<CommandRateConfig> config)
    {
        _config = config;
    }

    public override async IAsyncEnumerable<IResponse> OnMessageReceived(MessageContext context)
    {
        var commandLineResult = context.CommandLineResult;
        if (commandLineResult == null) yield break;

        var command = commandLineResult.Command.ToString()!;
        var plugin = context.NextPlugins.FirstOrDefault(k => k.Commands.ContainsKey(command));

        if (plugin == null) yield break;

        _config.Instance.CommandRate.AddOrUpdate(command, 1, (_, i) => i + 1);
        await _config.Instance.SaveAsync();
    }

    public sealed class CommandRateConfig : ConfigurationBase
    {
        public ConcurrentDictionary<string, int> CommandRate { get; set; } = new();
    }
}