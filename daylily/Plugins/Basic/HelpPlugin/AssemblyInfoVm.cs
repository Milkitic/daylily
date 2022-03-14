namespace daylily.Plugins.Basic.HelpPlugin;

public class AssemblyInfoVm
{
    public AssemblyInfoVm(string name, IReadOnlyList<PluginInfoVm> pluginInfos)
    {
        Name = name;
        PluginInfos = pluginInfos;
    }

    public string Name { get; }
    public IReadOnlyList<PluginInfoVm> PluginInfos { get; }
}