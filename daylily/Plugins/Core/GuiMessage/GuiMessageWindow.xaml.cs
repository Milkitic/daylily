using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Channels;
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
using MilkiBotFramework.Connecting;
using MilkiBotFramework.ContactsManaging;
using MilkiBotFramework.ContactsManaging.Models;
using MilkiBotFramework.Data;
using static ICSharpCode.AvalonEdit.Document.TextDocumentWeakEventManager;

namespace daylily.Plugins.Core.GuiMessage;

public class GuiMessageWindowVm : ViewModelBase
{
    private ObservableCollection<ChannelInfo> _allChannels = new();
    private ObservableCollection<PrivateInfo> _allPrivates = new();
    private ChannelInfo? _selectedChannel;

    public ObservableCollection<ChannelInfo> AllChannels
    {
        get => _allChannels;
        set => SetField(ref _allChannels, value);
    }

    public ObservableCollection<PrivateInfo> AllPrivates
    {
        get => _allPrivates;
        set => SetField(ref _allPrivates, value);
    }

    public ChannelInfo? SelectedChannel
    {
        get => _selectedChannel;
        set => SetField(ref _selectedChannel, value);
    }
}

/// <summary>
/// GuiMessageWindow.xaml 的交互逻辑
/// </summary>
public partial class GuiMessageWindow : Window
{
    private readonly IContactsManager _contactsManager;
    private readonly IMessageApi _messageApi;
    private readonly Bot _bot;
    private readonly GuiMessageWindowVm _viewModel;

    private readonly HashSet<PrivateInfo> _privateInfos = new();
    private readonly HashSet<ChannelInfo> _channelInfos = new();
    public GuiMessageWindow(IContactsManager contactsManager, IMessageApi messageApi, Bot bot)
    {
        InitializeComponent();
        DataContext = _viewModel = new GuiMessageWindowVm();
        _contactsManager = contactsManager;
        _messageApi = messageApi;
        _bot = bot;
    }

    private void GuiMessageWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        new Task(() =>
        {
            while (true)
            {
                var channels = _contactsManager.GetAllChannels();
                var newChannels = new HashSet<ChannelInfo>(channels);
                foreach (var channel in _channelInfos.Where(channel => !newChannels.Contains(channel)))
                {
                    _channelInfos.Remove(channel);
                    Dispatcher.Invoke(() => _viewModel.AllChannels.Remove(channel));
                }

                foreach (var channel in newChannels.Where(channel => !_channelInfos.Contains(channel)))
                {
                    _channelInfos.Add(channel);
                    Dispatcher.Invoke(() => _viewModel.AllChannels.Add(channel));
                }

                var privates = _contactsManager.GetAllPrivates();
                var newPrivates = new HashSet<PrivateInfo>(privates);
                foreach (var privateInfo in _privateInfos.Where(channel => !newPrivates.Contains(channel)))
                {
                    _privateInfos.Remove(privateInfo);
                    Dispatcher.Invoke(() => _viewModel.AllPrivates.Remove(privateInfo));
                }

                foreach (var privateInfo in newPrivates.Where(channel => !_privateInfos.Contains(channel)))
                {
                    _privateInfos.Add(privateInfo);
                    Dispatcher.Invoke(() => _viewModel.AllPrivates.Add(privateInfo));
                }

                Thread.Sleep(5000);
            }
        }).Start();
    }

    private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        await SendMessage();
    }

    private async void TbMessage_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            await SendMessage();
        }
        else
        {
            TbMessage.Text += Environment.NewLine;
        }

        e.Handled = true;
    }

    private async Task SendMessage()
    {
        var message = TbMessage.Text;
        if (string.IsNullOrEmpty(message)) return;
        if (_viewModel.SelectedChannel is not { } channelInfo) return;
        await _messageApi.SendChannelMessageAsync(channelInfo.ChannelId, message, null, null, null);
        TbMessage.Text = "";
    }
}