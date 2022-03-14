using System.Windows;
using MilkiBotFramework.Imaging.Wpf.Internal;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins.Core.GuiManaging;

[PluginIdentifier("18bd9084-ece9-48e9-8c2d-91e26ca494de")]
public class GuiManager : ServicePlugin
{
    private readonly PluginManager _pluginManager;

    public GuiManager(PluginManager pluginManager)
    {
        _pluginManager = pluginManager;
    }

    protected override async Task OnInitialized()
    {
        await UiThreadHelper.EnsureUiThreadAsync();
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var managerWindow = new ManagerWindow(_pluginManager.GetAllPlugins());
            managerWindow.Show();
        });
    }
}