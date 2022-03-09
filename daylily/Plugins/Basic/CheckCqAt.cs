using System.ComponentModel;
using MilkiBotFramework.ContactsManaging;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Messaging.RichMessages;
using MilkiBotFramework.Platforms.GoCqHttp.Messaging.CqCodes;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins.Basic;

[PluginIdentifier("e6d765b3-a015-4192-9cc1-0cfa5c13ec55", "@检测")]
[PluginLifetime(PluginLifetime.Scoped)]
[Description("当自己被at时回击at对方")]
public class CheckCqAt : BasicPlugin
{
    private readonly IContactsManager _contactsManager;

    public CheckCqAt(IContactsManager contactsManager)
    {
        _contactsManager = contactsManager;
    }

    public override async IAsyncEnumerable<IResponse> OnMessageReceived(MessageContext context)
    {
        if (context.MessageIdentity?.MessageType != MessageType.Channel) yield break;
        var richMsg = context.GetRichMessage();
        var allAts = richMsg
            .Where(k => k is CQAt)
            .Select(k => ((CQAt)k).UserId)
            .ToHashSet();
        var result = await _contactsManager.TryGetOrUpdateSelfInfo();
        if (!result.IsSuccess) yield break;
        if (!allAts.Contains(result.SelfInfo!.UserId) && !allAts.Contains("-1")) yield break;

        await Task.Delay(Random.Shared.Next(5000));
        if (Random.Shared.NextDouble() < 0.9)
        {
            yield return Reply(new At(context.MessageUserIdentity!.UserId), false);
        }
        else
        {
            var imagePath = Path.Combine(PluginHome, "pandas", "at.jpg");
            yield return Reply(new FileImage(imagePath), false);
        }
    }
}