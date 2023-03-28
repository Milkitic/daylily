using daylily.Utils;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins.Services;

[PluginIdentifier("c7812045-5b4c-44a3-af71-f0fedf73cc1f")]
public sealed class FailExecuteReplyService : ServicePlugin
{
    public override async Task<IResponse?> OnPluginException(Exception exception, MessageContext context)
    {
        if (context.Authority != MessageAuthority.Root)
        {
            return Reply("指令执行出错！" +
                         "\r\n错误信息：" + (exception?.Message));
        }

        var message = "指令执行出错！" +
                      "\r\n错误信息：" + (exception?.ToFullTypeMessage());

        return Reply(message);
    }
}