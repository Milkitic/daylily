using System.ComponentModel;
using Microsoft.Extensions.Logging;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins.Core;

[PluginIdentifier("9a71f0a2-fd2e-4d7e-abd8-681d14d0d83e", "控制台消息输出", AllowDisable = false)]
[PluginLifetime(PluginLifetime.Singleton)]
[Author("milkiyf")]
[Description("用于后台DEBUG")]
public class CliPrint : BasicPlugin
{
    private readonly ILogger<CliPrint> _logger;

    public CliPrint(ILogger<CliPrint> logger)
    {
        _logger = logger;
    }

    public override IAsyncEnumerable<IResponse> OnMessageReceived(MessageContext context)
    {
        var identity = context.MessageIdentity!;
        string session = "未知会话";
        string sender = "未知发送者";

        if (identity.MessageType == MessageType.Private)
        {
            var privateInfo = context.PrivateInfo!;
            session = "私聊 " + privateInfo.UserId + " - " +
                      (privateInfo.Remark ?? privateInfo.Nickname ?? privateInfo.UserId);
            sender = "对方";
        }
        else if (identity.MessageType == MessageType.Channel)
        {
            var channelInfo = context.ChannelInfo!;
            var memberInfo = context.MemberInfo!;
            session = identity.SubId == null
                ? $"群 {channelInfo.ChannelId} - {channelInfo.Name}"
                : $"频道 {channelInfo.ChannelId}.{channelInfo.SubChannelId} - {channelInfo.Name}";
            var name = memberInfo.Card ?? memberInfo.Nickname ?? memberInfo.UserId;
            sender = name == memberInfo.UserId ? name : name + $" ({memberInfo.UserId})";
        }

        var richMessage = context.GetRichMessage().ToString();
        var actualMessage = string.Join('\n', richMessage.Split('\n').Select(k => "  " + k));

        _logger.LogInformation($"{context.ReceivedTime.LocalDateTime} ({session}) {sender}:\r\n" +
                               $"{actualMessage}");

        return base.OnMessageReceived(context);
    }
}