using Microsoft.Extensions.Logging;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Plugining.Loading;

namespace daylily.Plugins.Services;

[PluginIdentifier("440f5374-925e-4065-85cd-c06e0175af06", Index = 99)]
public class CliPrintMe : ServicePlugin
{
    private readonly ILogger _logger;

    public CliPrintMe(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger("raw_me");
    }

    public override async Task<bool> BeforeSend(PluginInfo pluginInfo, IResponse response)
    {
        if (response.Message == null) return true;

        var context = response.MessageContext;
        var identity = context?.MessageIdentity;
        string session = "未知会话";

        if (context != null && identity != null)
        {
            if (identity.MessageType == MessageType.Private)
            {
                var privateInfo = context.PrivateInfo!;
                session = "私聊 " + privateInfo.UserId + " - " +
                          (privateInfo.Remark ?? privateInfo.Nickname ?? privateInfo.UserId);
            }
            else if (identity.MessageType == MessageType.Channel)
            {
                var channelInfo = context.ChannelInfo!;
                session = identity.SubId == null
                    ? $"群 {channelInfo.ChannelId} - {channelInfo.Name}"
                    : $"频道 {channelInfo.ChannelId}.{channelInfo.SubChannelId} - {channelInfo.Name}";
            }
        }

        var time = DateTime.Now;
        var richMessage = response.Message.ToString()!;
        var actualMessage = string.Join('\n', richMessage.Split('\n').Select(k => "  " + k));

        _logger.LogInformation($"{time} ({session}) 我:\r\n" +
                               $"{actualMessage}");
        return true;
    }
}