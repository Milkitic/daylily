using MilkiBotFramework.Imaging;
using MilkiBotFramework.Imaging.Wpf;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Messaging.RichMessages;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Plugining.Loading;

namespace daylily.Plugins.Basic.HelpPlugin;

[PluginIdentifier("641dbf44-4a1b-4444-907e-c608ac2b1cfc", "Help选单")]
public class Help : BasicPlugin
{
    private readonly PluginManager _pluginManager;

    public Help(PluginManager pluginManager)
    {
        _pluginManager = pluginManager;
    }

    [CommandHandler("help")]
    public Task<IResponse> GetHelp(MessageContext context,
        [Option("app")] bool includeNonCommand = false,
        [Argument] string? pluginName = null)
    {
        if (pluginName == null) return GetHelpList(context, includeNonCommand);
        throw new NotImplementedException();
    }

    private async Task<IResponse> GetHelpList(MessageContext context, bool includeNonCommand)
    {
        IEnumerable<PluginInfo> plugins = _pluginManager.GetAllPlugins();
        if (!includeNonCommand)
            plugins = plugins.Where(k => k.Commands.Count != 0);

        var assemblyInfoVms = plugins
            .GroupBy(k => k.Metadata.Scope)
            .Where(k => k.Any())
            .Select(pluginInfos =>
            {
                return new ScopeInfoVm(pluginInfos.Key, pluginInfos
                    .Select(k => (pluginInfo: k,
                        commands: k.Commands
                            .Where(o => o.Value.Authority <= context.Authority &&
                                        o.Value.MessageType.HasFlag(context.MessageIdentity!.MessageType))
                            .Select(o => o.Value)
                            .OrderBy(o => o.Command)
                            .ToArray())
                    )
                    .Where(k => k.commands.Length != 0)
                    .Select(k => new PluginInfoVm(k.pluginInfo, k.commands))
                    .OrderBy(k => k.Commands[0].Command)
                    .ToArray());
            })
            .OrderBy(k => k.Scope)
            .ToArray();
        var renderer = new WpfDrawingProcessor<HelpListVm, HelpListControl>(true);

        var helpViewModel = new HelpListVm(assemblyInfoVms);
        var image = await renderer.ProcessAsync(helpViewModel);
        return Reply(new MemoryImage(image, ImageType.Png), false);
    }
}