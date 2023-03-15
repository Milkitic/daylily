using System.Reflection;

namespace daylily.Plugins.Basic.HelpPlugin;

public sealed class HelpDetailVm
{
    public HelpDetailVm(PluginDetailVm pluginVm,char commandFlag)
    {
        PluginVm = pluginVm;
        CommandFlag = commandFlag;
    }

    public string AppName { get; } = Assembly.GetEntryAssembly()?.GetName().Name ?? "DynamicBot";

    public string VersionString { get; } = Assembly.GetEntryAssembly()
        ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.1-alpha";

    public string CoreVersionString { get; } = typeof(MilkiBotFramework.Bot).Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.1-alpha";

    public PluginDetailVm PluginVm { get; }

    public char CommandFlag { get; }
    //= new()
    //{
    //    Name = "那没事了",
    //    Helps = new List<string>() { "demo help", "multi sentences" },
    //    Commands = new List<CommandInfo>()
    //    {
    //        new CommandDefinition("nmsl", "你聋了", false),
    //        new CommandDefinition("wslnm", "龙图小将", true),
    //    },
    //    FreeArgs = new List<StringKeyValuePair>()
    //    {
    //        new StringKeyValuePair() { Key = "image", Value = null },
    //    },
    //    Args = new List<StringKeyValuePair>()
    //    {
    //        new StringKeyValuePair() { Key = "x", Value = "抖动X范围" },
    //        new StringKeyValuePair() { Key = "y", Value = "抖动Y范围" }
    //    },
    //    Authors = new[] { "lyt555", "test" },
    //    Regexes = new List<RegexDefinition>()
    //    {
    //        new RegexDefinition("test", "nmsl")
    //    },
    //    State = PluginVersion.Alpha,
    //    Version = "1.0.0",
    //    Usage = "[-x x_range] [image]",
    //    CommandFlag = AppSettings.Default?.BotSettings.CommandFlag,
    //    DefaultCommand = "lyt555"
    //};
}