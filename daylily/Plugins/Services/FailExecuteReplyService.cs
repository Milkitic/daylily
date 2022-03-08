using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins.Services;

[PluginIdentifier("c7812045-5b4c-44a3-af71-f0fedf73cc1f")]
public sealed class FailExecuteReplyService : ServicePlugin
{
    public override async Task<IResponse?> OnPluginException(Exception exception, MessageContext messageContext)
    {
        if (messageContext.Authority != MessageAuthority.Root) return null;

#if DEBUG
        var message = "插件执行出错啦！调试信息：\r\n" + exception;
#else
        var message = "插件执行出错：" + exception.Message;
#endif
        return Reply(message);
    }
}