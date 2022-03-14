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
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Loading;

namespace daylily.Plugins.Core.GuiManaging;

/// <summary>
/// ManagerWindow.xaml 的交互逻辑
/// </summary>
internal partial class ManagerWindow : Window
{
    public IReadOnlyList<PluginInfo> Plugins { get; }

    public ManagerWindow(IReadOnlyList<PluginInfo> plugins)
    {
        Plugins = plugins;
        InitializeComponent();
    }

    private void ManagerWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        e.Cancel = true;
    }

    private void ButtonOpenHome_OnClick(object sender, RoutedEventArgs e)
    {
        var homePath = (string)((Button)sender).Tag;
        var fullPath = System.IO.Path.GetFullPath(homePath);
        Process.Start("explorer.exe", fullPath);
        //Process.Start("explorer.exe", $"/select,\"{fullPath}\"");
    }
}