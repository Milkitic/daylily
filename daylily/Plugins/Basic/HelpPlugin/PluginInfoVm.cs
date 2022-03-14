using MilkiBotFramework.Plugining.Loading;

namespace daylily.Plugins.Basic.HelpPlugin;

public class PluginInfoVm
{
    public PluginInfoVm(PluginInfo pluginInfo, IReadOnlyList<CommandInfo> commands)
    {
        PluginInfo = pluginInfo;
        Commands = commands;
    }

    public PluginInfo PluginInfo { get; }
    public IReadOnlyList<CommandInfo> Commands { get; }
}