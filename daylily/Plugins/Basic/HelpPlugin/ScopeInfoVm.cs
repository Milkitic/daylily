namespace daylily.Plugins.Basic.HelpPlugin;

public sealed class ScopeInfoVm
{
    public ScopeInfoVm(string scope, IReadOnlyList<PluginInfoVm> pluginInfos)
    {
        Scope = scope;
        PluginInfos = pluginInfos;
    }

    public string Scope { get; }
    public IReadOnlyList<PluginInfoVm> PluginInfos { get; }
}