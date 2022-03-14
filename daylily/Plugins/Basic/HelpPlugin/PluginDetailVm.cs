using MilkiBotFramework.Plugining.Loading;

namespace daylily.Plugins.Basic.HelpPlugin;

public class PluginDetailVm
{
    public PluginDetailVm(string name, string? description, string authors, PluginInfo pluginInfo,
        IReadOnlyCollection<CommandInfo> commands, CommandInfo? currentCommand)
    {
        Name = name;
        Description = description;
        Authors = authors;
        PluginInfo = pluginInfo;
        Commands = commands;
        CurrentCommand = currentCommand;
    }

    public string Name { get; }
    public string? Description { get; }
    public string Authors { get; }
    //public List<RegexDefinition> Regexes { get; set; }
    public PluginInfo PluginInfo { get; }
    public IReadOnlyCollection<CommandInfo> Commands { get; }
    public CommandInfo? CurrentCommand { get; }
    public string? CurrentCommandUsage { get; set; }
    public IReadOnlyList<CommandParameterInfo> CurrentArguments { get; set; } = Array.Empty<CommandParameterInfo>();
    public IReadOnlyList<CommandParameterInfo> CurrentOptions { get; set; } = Array.Empty<CommandParameterInfo>();
}