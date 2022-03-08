using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins.Services;

[PluginIdentifier("f3561463-fda7-4d97-a899-50f6ba4a36ac")]
public sealed class FailBindingReplyService : ServicePlugin
{
    public override async Task<IResponse?> OnBindingFailed(BindingException bindingException, MessageContext context)
    {
        if (bindingException.BindingFailureType == BindingFailureType.AuthenticationFailed)
            return Reply(DefaultReply.RootOnly);
        if (bindingException.BindingFailureType == BindingFailureType.MessageTypeFailed)
            return Reply(DefaultReply.PrivateOnly);
        return null;
    }
}