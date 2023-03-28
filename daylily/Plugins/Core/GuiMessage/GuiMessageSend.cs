using System.Windows;
using MilkiBotFramework;
using MilkiBotFramework.Connecting;
using MilkiBotFramework.ContactsManaging;
using MilkiBotFramework.Imaging.Wpf.Internal;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins.Core.GuiMessage;

[PluginIdentifier("8bb00a92-a2ce-4985-93c6-48d2958f1a97")]
public class GuiMessageSend : ServicePlugin
{
    private readonly Bot _bot;
    private readonly IContactsManager _contactsManager;
    private readonly IMessageApi _messageApi;

    public GuiMessageSend(IContactsManager contactsManager, IMessageApi messageApi, Bot bot)
    {
        _contactsManager = contactsManager;
        _messageApi = messageApi;
        _bot = bot;
    }

    protected override async Task OnInitialized()
    {
        await UiThreadHelper.EnsureUiThreadAsync();
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var guiMessageWindow = new GuiMessageWindow(_contactsManager, _messageApi, _bot);
            guiMessageWindow.Show();
        });
    }
}