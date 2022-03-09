using System.ComponentModel;
using Microsoft.Extensions.Logging;
using MilkiBotFramework.ContactsManaging;
using MilkiBotFramework.ContactsManaging.Models;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Platforms.GoCqHttp.Connecting;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins;

[PluginIdentifier("4c729d16-3954-4e70-ad4c-8a0ea72efe1a", "自助禁言")]
[Description("当${BotName}是管理员时，将命令发送者禁言（30分钟到12小时）。")]
public class Smoke : BasicPlugin
{
    private readonly IContactsManager _contactsManager;
    private readonly ILogger<Smoke> _logger;
    private readonly GoCqApi _goCqApi;

    public Smoke(IContactsManager contactsManager, ILogger<Smoke> logger, GoCqApi goCqApi)
    {
        _contactsManager = contactsManager;
        _logger = logger;
        _goCqApi = goCqApi;
    }

    [CommandHandler("sleep", AllowedMessageType = MessageType.Channel)]
    public async Task<IResponse> SmokeHandler(MessageContext context,
        [Argument, Description("要禁言的时长，小时为单位，支持小数")] double sleepTime = 0)
    {
        var userIdentity = context.MessageUserIdentity;
        var messageIdentity = userIdentity!.MessageIdentity;
        var userId = userIdentity.UserId;
        var channelId = messageIdentity.Id!;

        var self = await _contactsManager.TryGetOrUpdateSelfInfo();
        if (self.IsSuccess)
        {
            var groupMember = await _contactsManager.TryGetOrAddMemberInfo(
                channelId, self.SelfInfo!.UserId, messageIdentity.SubId);
            if (groupMember.IsSuccess)
            {
                if (groupMember.MemberInfo!.MemberRole == MemberRole.Member)
                {
                    return Reply("${BotName}不是管理员，没办法自助禁言o");
                }
            }
        }

        if (sleepTime == 0)
        {
            return Reply("要禁多少小时？");
        }

        if (sleepTime > 12)
        {
            sleepTime = 12;
        }
        else if (sleepTime < 0.5)
        {
            sleepTime = 0.5;
        }
        else if (sleepTime > 0)
        {
            //ignore
        }
        else
        {
            return Reply("处于4维时空的我们，可不允许在时间轴上走回头路..");
        }

        var totalTime = TimeSpan.FromHours(sleepTime);
        try
        {
            await _goCqApi.SetGroupBan(long.Parse(channelId), long.Parse(userId), totalTime);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "禁言时出错");
            return Reply("由于不可抗力，${BotName}没有办法禁言..");
        }

        return Reply($"祝你一觉睡到{DateTime.Now.AddHours(sleepTime):HH:mm} 🙂");
    }
}