using System.Text;
using daylily.Utils;
using MilkiBotFramework;
using MilkiBotFramework.Imaging;
using MilkiBotFramework.Imaging.Wpf;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Messaging.RichMessages;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Plugining.Loading;

namespace daylily.Plugins.Basic.HelpPlugin;

[PluginIdentifier("641dbf44-4a1b-4444-907e-c608ac2b1cfc", "Help选单")]
[PluginLifetime(PluginLifetime.Singleton)]
public sealed class Help : BasicPlugin
{
    private readonly PluginManager _pluginManager;
    private readonly Dictionary<string, (PluginInfo, bool)> _detailMapping = new();

    public Help(PluginManager pluginManager)
    {
        _pluginManager = pluginManager;
    }

    protected override async Task OnInitialized()
    {
        var allPlugins = _pluginManager.GetAllPlugins();

        foreach (var pluginInfo in allPlugins)
        {
            foreach (var (key, _) in pluginInfo.Commands)
            {
                if (!_detailMapping.ContainsKey(key))
                    _detailMapping.Add(key, (pluginInfo, true));
            }

            if (!_detailMapping.ContainsKey(pluginInfo.Metadata.Name))
                _detailMapping.Add(pluginInfo.Metadata.Name, (pluginInfo, false));
        }
    }

    [CommandHandler("help")]
    public async Task<IResponse> GetHelp(MessageContext context,
        [Option("app")] bool includeNonCommand = false,
        [Argument] string? pluginName = null)
    {
        if (pluginName == null) return Reply(await GetHelpList(context, includeNonCommand), false);
        return Reply(await GetDetailList(context, pluginName), false);
    }

    private async Task<IRichMessage> GetHelpList(MessageContext context, bool includeNonCommand)
    {
        IEnumerable<PluginInfo> plugins = _pluginManager.GetAllPlugins();
        //if (!includeNonCommand)
        //    plugins = plugins.Where(k => k.Commands.Count != 0);

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
        return new MemoryImage(image, ImageType.Png);
    }

    private async Task<IRichMessage> GetDetailList(MessageContext context, string pluginName)
    {
        if (!_detailMapping.TryGetValue(pluginName, out var tuple))
        {
            return await GetHelpList(context, true) +
                   new Text("\r\n未找到对应插件哦，请查看以上列表");
        }

        var pluginInfo = tuple.Item1;
        var fromCommand = tuple.Item2;
        var pluginDetailVm = new PluginDetailVm(pluginInfo.Metadata.Name,
            pluginInfo.Metadata.Description,
            pluginInfo.Metadata.Authors,
            pluginInfo,
            pluginInfo.Commands.Values,
            fromCommand ? pluginInfo.Commands[pluginName] : null);

        if (pluginDetailVm.CurrentCommand != null)
        {
            var sbOptions = new StringBuilder();
            var sbOptionsSwitch = new StringBuilder();
            var sbArguments = new StringBuilder();
            var parameterInfos = pluginDetailVm.CurrentCommand.ModelBindingInfo?.ParameterInfos ??
                                 pluginDetailVm.CurrentCommand.ParameterInfos;
            var options = new List<CommandParameterInfo>();
            var arguments = new List<CommandParameterInfo>();
            foreach (var parameterInfo in parameterInfos)
            {
                if (parameterInfo.IsServiceArgument) continue;
                if (parameterInfo.Authority > context.Authority) continue;
                if (parameterInfo.IsArgument)
                {
                    sbArguments.Append($" [{parameterInfo.ParameterName.ToLowerSnake()}]");
                    arguments.Add(parameterInfo);
                }
                else
                {
                    if (parameterInfo.ParameterType == StaticTypes.Boolean)
                    {
                        sbOptionsSwitch.Append($" [{parameterInfo.Name}]");
                    }
                    else
                    {
                        sbOptions.Append($" [{parameterInfo.Name} {parameterInfo.ParameterName.ToLowerSnake()}]");
                    }

                    options.Add(parameterInfo);
                }
            }

            pluginDetailVm.CurrentCommandUsage = $"/{pluginName}{sbOptions}{sbArguments}{sbOptionsSwitch}";
            pluginDetailVm.CurrentArguments = arguments;
            pluginDetailVm.CurrentOptions = options;
        }

        var renderer = new WpfDrawingProcessor<HelpDetailVm, HelpDetailControl>(true);

        var helpViewModel = new HelpDetailVm(pluginDetailVm);
        var image = await renderer.ProcessAsync(helpViewModel);
        return new MemoryImage(image, ImageType.Png);
    }
}