using MilkiBotFramework;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins.Services;

[PluginIdentifier("f3561463-fda7-4d97-a899-50f6ba4a36ac")]
public sealed class FailBindingReplyService : ServicePlugin
{
    private readonly BotOptions _botOptions;

    public FailBindingReplyService(BotOptions botOptions)
    {
        _botOptions = botOptions;
    }

    public override async Task<IResponse?> OnBindingFailed(BindingException bindingException, MessageContext context)
    {
        if (bindingException.BindingFailureType == BindingFailureType.AuthenticationFailed)
        {
            if (context.Authority == MessageAuthority.Admin)
                return Reply("此功能需要开发者权限..");
            return Reply("此功能需要群内管理员权限..");
        }

        if (bindingException.BindingFailureType == BindingFailureType.MessageTypeFailed)
        {
            if (context.MessageIdentity!.MessageType == MessageType.Channel)
                return Reply("此功能仅限私聊..");
            return Reply("此功能仅限群聊..");
        }

        if (bindingException.BindingFailureType == BindingFailureType.Mismatch)
        {
            GetCommandInfo(bindingException, out var command, out var info, out var type);
            var message = $"指令缺少{type}：{info}。请使用 \"{_botOptions.CommandFlag}help {command}\" 查看说明。";
            message = AppendError(bindingException, context, message);

            return Reply(message);
        }

        if (bindingException.BindingFailureType == BindingFailureType.ConvertError)
        {
            GetCommandInfo(bindingException, out var command, out var info, out var type);
            var message = $"指令{type}解析出错：{info}。请使用 \"{_botOptions.CommandFlag}help {command}\" 查看说明。";
            message = AppendError(bindingException, context, message);

            return Reply(message);
        }

        return null;
    }

    private static void GetCommandInfo(BindingException bindingException, out string command, out string info, out string type)
    {
        var parameterInfo = bindingException.BindingSource.ParameterInfo!;
        command = bindingException.BindingSource.CommandInfo.Command;
        var retType = parameterInfo.ParameterType == StaticTypes.Boolean ? "" : " ...";
        info = parameterInfo.IsArgument ? $"\"{parameterInfo.ParameterName}\"" : $"\"-{parameterInfo.Name}{retType}\"";
        type = parameterInfo.IsArgument ? "参数" : "选项";
    }

    private static string AppendError(BindingException bindingException, MessageContext context, string message)
    {
        if (context.Authority != MessageAuthority.Root) return message;

//#if DEBUG
//        if (context.MessageIdentity!.MessageType == MessageType.Private)
//        {
//            message += "调试信息：\r\n" + (bindingException.InnerException ?? bindingException);
//        }
//        else
//#endif
        {
            message += "\r\n错误信息：" + (bindingException.InnerException?.Message ?? bindingException.Message);
        }

        return message;
    }
}