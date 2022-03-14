using System.Reflection;

namespace daylily.Plugins.Basic.HelpPlugin;

public sealed class HelpListVm
{
    public HelpListVm(IReadOnlyList<ScopeInfoVm> assemblyInfoVms)
    {
        AssemblyInfoVms = assemblyInfoVms;
    }

    public string AppName { get; } = Assembly.GetEntryAssembly()?.GetName().Name ?? "DynamicBot";

    public string VersionString { get; } = Assembly.GetEntryAssembly()
        ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.1-alpha";

    public string CoreVersionString { get; } = typeof(MilkiBotFramework.Bot).Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.1-alpha";

    public IReadOnlyList<ScopeInfoVm> AssemblyInfoVms { get; }
}