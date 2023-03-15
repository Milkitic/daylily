using System.ComponentModel;
using System.Text;
using MilkiBotFramework;
using MilkiBotFramework.ContactsManaging;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Plugining.Configuration;
using MilkiBotFramework.Plugining.Loading;

namespace daylily.Plugins.Core;

[PluginIdentifier("1888139a-860d-41f6-8684-639b2b6923e9", "插件管理", Index = -20, AllowDisable = false)]
[Description("动态管理插件的启用状态（仅限当前群生效）")]
[PluginLifetime(PluginLifetime.Singleton)]
public class PluginFilter : BasicPlugin
{
    private readonly IContactsManager _contactsManager;
    private readonly PluginManager _pluginManager;
    private readonly BotOptions _botOptions;
    private readonly PluginManagerConfig _config;

    private Dictionary<Guid, PluginInfo> _plugins;
    private Dictionary<string, PluginInfo> _pluginsMapping;

    public PluginFilter(IConfiguration<PluginManagerConfig> configuration,
        IContactsManager contactsManager,
        PluginManager pluginManager,
        BotOptions botOptions)
    {
        _contactsManager = contactsManager;
        _pluginManager = pluginManager;
        _botOptions = botOptions;
        _config = configuration.Instance;
    }

    public enum SubCommand
    {
        Enable, Disable, List
    }

    protected override async Task OnInitialized()
    {
        await _config.SaveAsync();
        _plugins = _pluginManager
            .GetAllPlugins()
            .Where(k => k.PluginType == PluginType.Basic && k.AllowDisable)
            .ToDictionary(k => k.Metadata.Guid, k => k);
        _pluginsMapping = _plugins.Values
            .SelectMany(k => k.Commands.Keys.Select(o => (o, k)))
            .ToDictionary(k => k.o, k => k.k);
    }

    public override async IAsyncEnumerable<IResponse> OnMessageReceived(MessageContext context)
    {
        var identity = context.MessageIdentity!;
        var disabledDictionary = _config.IdentityDisabledDictionary;
        if (!disabledDictionary.TryGetValue(identity, out var disabled)) yield break;

        var allDisabled = disabled
            .Select(k => _plugins.TryGetValue(k, out var value) ? value : null);

        foreach (var info in allDisabled)
        {
            context.NextPlugins.Remove(info);
        }

        var contextCommandLineResult = context.CommandLineResult;
        if (contextCommandLineResult?.Command == null)
        {
            yield break;
        }

        var command = contextCommandLineResult.Command.ToString()!;
        if (!_pluginsMapping.TryGetValue(command, out var pluginInfo))
        {
            yield break;
        }

        if (disabled.Contains(pluginInfo.Metadata.Guid))
        {
            yield return identity.MessageType == MessageType.Private
                ? Reply("你已禁用此命令..")
                : Reply("本群已禁用此命令..");
        }
    }

    [CommandHandler(Authority = MessageAuthority.Admin)]
    public async Task<IResponse> Plugin(MessageContext context,
        [Argument] SubCommand commandType,
        [Option("d")] bool disableOnly = false,
        [Option("e")] bool enableOnly = false,
        [Argument] string? pluginNames = null)
    {
        var identity = context.MessageIdentity!;
        var disabledDictionary = _config.IdentityDisabledDictionary;
        var disabled = disabledDictionary.GetOrAdd(identity, new HashSet<Guid>());
        return commandType switch
        {
            SubCommand.Enable => await SwitchPlugin(disabled, pluginNames, true),
            SubCommand.Disable => await SwitchPlugin(disabled, pluginNames, false),
            SubCommand.List => await ListPlugins(context, disabled, enableOnly ? true : disableOnly ? false : null),
            _ => throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null)
        };
    }

    private async Task<IResponse> SwitchPlugin(HashSet<Guid> disabled, string? pluginNames, bool pluginState)
    {
        if (string.IsNullOrWhiteSpace(pluginNames))
            return Reply($"请指定插件名称或者命令名称..请使用 \"{_botOptions.CommandFlag}plugin list\" 查看插件列表");
        var inputs = pluginNames.Split(',');
        var sb = new StringBuilder();
        var dict = inputs.Distinct().ToDictionary(k => k, InnerGetPlugin);
        var targetStateName = pluginState ? "启用" : "禁用";
        var targetStateRev = pluginState ? "禁用" : "启用";

        foreach (var (input, plugin) in dict)
        {
            if (dict.Count != 1)
            {
                sb.Append(input + ": ");
            }

            if (plugin == null)
            {
                sb.Append($"指定插件 \"{input}\" 不存在..");
                if (dict.Count == 1) sb.Append($"请使用 \"{_botOptions.CommandFlag}plugin list\" 查看插件列表");
                else sb.AppendLine();
            }
            else if (pluginState && !disabled.Contains(plugin.Metadata.Guid))
                sb.AppendLine($"指定插件 \"{GetPluginString(plugin)}\" 未被{targetStateRev}..");
            else if (!pluginState && disabled.Contains(plugin.Metadata.Guid))
                sb.AppendLine($"指定插件 \"{GetPluginString(plugin)}\" 已被{targetStateName}..");
            else
            {
                if (pluginState)
                    disabled.Remove(plugin.Metadata.Guid);
                else
                    disabled.Add(plugin.Metadata.Guid);

                sb.AppendLine($"已经{targetStateName}插件 \"{GetPluginString(plugin)}\".");
            }
        }

        await _config.SaveAsync();
        return Reply(sb.ToString().Trim('\r', '\n'));
    }

    private async Task<IResponse> ListPlugins(MessageContext context, HashSet<Guid> disabled, bool? showState = null)
    {
        var id = context.MessageIdentity!;
        var name = id.MessageType == MessageType.Private
            ? "私聊"
            : await _contactsManager.GetIdentityName(id) + " ";

        var sb = new StringBuilder();
        sb.AppendLine(name + "的插件情况：");

        var pluginGroup = _plugins.Values.GroupBy(k => k.PluginType);
        foreach (var plugins in pluginGroup)
        {
            foreach (var plugin in plugins)
            {
                var isDisabled = disabled.Contains(plugin.Metadata.Guid);
                if (showState == false && !isDisabled ||
                    showState == true && isDisabled)
                {
                    continue;
                }

                var symbol = isDisabled ? "❌" : "✅";
                sb.AppendLine($"{symbol} {GetPluginString(plugin)}");
            }
        }

        if (id.MessageType != MessageType.Private)
        {
            sb.Append("（为避免消息过长，本条消息为私聊发送）");
            return ToPrivate(context.MessageUserIdentity!.UserId, sb.ToString());
        }

        return ToPrivate(context.MessageUserIdentity!.UserId, sb.ToString().TrimEnd('\n', '\r'));
    }

    private PluginInfo? InnerGetPlugin(string key)
    {
        var pluginInfo = _plugins.Values
            .FirstOrDefault(k => k.Metadata.Name == key ||
                                 k.Type.Name == key ||
                                 k.Commands.ContainsKey(key));
        return pluginInfo;
    }

    private static string GetPluginString(PluginInfo plugin)
    {
        return $"{plugin.Metadata.Name} ({plugin.Type.Name})";
    }
}