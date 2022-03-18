using System.ComponentModel;
using MilkiBotFramework.ContactsManaging;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins.Core;

[PluginIdentifier("09983821-0238-4b0d-b1c1-2921eb7e52d1", "发送自定义消息", AllowDisable = false)]
[Description("支持发送任意格式的消息（包含cq码）。")]
public class SendMessage : BasicPlugin
{
    private readonly IContactsManager _contactsManager;

    public SendMessage(IContactsManager contactsManager)
    {
        _contactsManager = contactsManager;
    }

    [CommandHandler(AllowedMessageType = MessageType.Private, Authority = MessageAuthority.Root)]
    public async IAsyncEnumerable<IResponse> Broadcast(MessageContext messageContext,
        [Argument, Description("要发送的信息。")]
        string? message = null)
    {
        var response = ValidateInput(message, out var innerMessage);
        if (response != null)
        {
            yield return response;
            yield break;
        }

        var groups = _contactsManager.GetAllChannels();

        string ok = $"◈◈ {DateTime.Now:M月d日 H:mm}公告 ◈◈\r\n";
        string titledMessage = ok + innerMessage;

        bool hasResult = false;
        int i = 0;
        foreach (var groupInfo in groups)
        {
            hasResult = true;

            var id = groupInfo.ChannelId;
            yield return ToChannel(id, titledMessage);
            i++;
            await Task.Delay(3000);
        }

        if (!hasResult)
        {
            yield return Reply("无有效群。");
        }
        else
        {
            //SaveLogs(msg, "announcement");
            yield return Reply($"已成功发送至{i}个群。");
        }
    }

    [CommandHandler(AllowedMessageType = MessageType.Private, Authority = MessageAuthority.Root)]
    public IResponse Send(MessageContext messageContext,
        [Option("g"), Description("要发送的群号。")]
        string? groupId = null,
        [Option("u"), Description("要发送的用户QQ号。")]
        string? userId = null,
        [Argument, Description("要发送的信息。")]
        string? message = null)
    {
        var response = ValidateInput(message, out var innerMessage);
        if (response != null)
            return response;
        if (groupId != null && userId != null)
            return Reply("请指定唯一一个目标");
        if (groupId != null)
            return ToChannel(groupId, innerMessage);
        if (userId != null)
            return ToPrivate(userId, innerMessage);
        return Reply(innerMessage);
    }

    private static IResponse? ValidateInput(string? message, out string innerMessage)
    {
        if (message == null)
        {
            innerMessage = string.Empty;
            return Reply("你要说什么……");
        }

        innerMessage = Decode(message);
        return null;
    }

    private static string Decode(string source) =>
        source.Replace("\\&amp;", "&tamp;").Replace("\\#91;", "&t#91;").Replace("\\&#93;", "&t#93;").Replace("\\&#44;", "&t#44;")
            .Replace("&amp;", "&").Replace("&#91;", "[").Replace("&#93;", "]").Replace("&#44;", ",").
            Replace("&tamp;", "&amp;").Replace("&t#91;", "&#91;").Replace("&t#93;", "&#93;").Replace("&t#44;", "&#44;");
}