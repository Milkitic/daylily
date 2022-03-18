using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MilkiBotFramework;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Loading;

namespace daylily.Plugins.Core.GuiManaging;

/// <summary>
/// ManagerWindow.xaml 的交互逻辑
/// </summary>
internal partial class ManagerWindow : Window
{
    private readonly Bot _bot;
    public IReadOnlyList<PluginInfo> Plugins { get; }

    public ManagerWindow(Bot bot, IReadOnlyList<PluginInfo> plugins)
    {
        _bot = bot;
        Plugins = plugins;
        InitializeComponent();
    }

    private async void ManagerWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        var yesNo = MessageBox.Show("exit?", "?", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (yesNo != MessageBoxResult.Yes)
        {
            e.Cancel = true;
        }
        else
        {
            await _bot.StopAsync();
        }
    }

    private void ButtonOpenHome_OnClick(object sender, RoutedEventArgs e)
    {
        var homePath = (string)((Button)sender).Tag;
        var fullPath = System.IO.Path.GetFullPath(homePath);
        Process.Start("explorer.exe", fullPath);
        //Process.Start("explorer.exe", $"/select,\"{fullPath}\"");
    }
}