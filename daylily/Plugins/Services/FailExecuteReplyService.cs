using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins.Services;

[PluginIdentifier("c7812045-5b4c-44a3-af71-f0fedf73cc1f")]
public sealed class FailExecuteReplyService : ServicePlugin
{
    public override async Task<IResponse?> OnPluginException(Exception exception, MessageContext context)
    {
        if (context.Authority != MessageAuthority.Root) return null;

        var message = "插件执行出错啦！";
//#if DEBUG
//        if (context.MessageIdentity!.MessageType == MessageType.Private)
//        {
//            message += "调试信息：\r\n" + (exception);
//        }
//        else
//#endif
        {
            message += "\r\n错误信息：" + (exception?.Message);
        }

        return Reply(message);
    }
}