using System.Windows;
using MilkiBotFramework;
using MilkiBotFramework.Imaging.Wpf.Internal;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins.Core.GuiManaging;

[PluginIdentifier("18bd9084-ece9-48e9-8c2d-91e26ca494de")]
public class GuiManager : ServicePlugin
{
    private readonly Bot _bot;
    private readonly PluginManager _pluginManager;

    public GuiManager(Bot bot, PluginManager pluginManager)
    {
        _bot = bot;
        _pluginManager = pluginManager;
    }

    protected override async Task OnInitialized()
    {
        await UiThreadHelper.EnsureUiThreadAsync();
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var managerWindow = new ManagerWindow(_bot, _pluginManager.GetAllPlugins());
            managerWindow.Show();
        });
    }
}