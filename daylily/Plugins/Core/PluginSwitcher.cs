using System.ComponentModel;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Plugining.Configuration;
using MilkiBotFramework.Plugining.Loading;

namespace daylily.Plugins.Core;

[PluginIdentifier("1888139a-860d-41f6-8684-639b2b6923e9", "插件管理", Index = -20)]
[Author("milkiyf")]
[Version("3.0.0")]
[Description("动态管理插件的启用状态（仅限当前群生效）")]
[PluginLifetime(PluginLifetime.Singleton)]
public class PluginSwitcher : BasicPlugin
{
    private readonly PluginManager _pluginManager;
    private readonly PluginManagerConfig _config;

    private IReadOnlyList<PluginInfo> _plugins;

    public PluginSwitcher(IConfiguration<PluginManagerConfig> configuration, PluginManager pluginManager)
    {
        _pluginManager = pluginManager;
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
            .Where(k => !k.IsService && k.AllowDisable)
            .ToArray();
    }

    [CommandHandler(Authority = MessageAuthority.Admin)]
    public IResponse Plugin([Argument] SubCommand subCommand,
        [Option("d")] bool disableOnly = false,
        [Option("e")] bool enableOnly = false,
        [Argument] string? pluginName = null)
    {
        return null;
    }
}