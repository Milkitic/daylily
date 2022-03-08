using System.ComponentModel;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins;

[Author("milkiyf")]
[Description("ping-pong!")]
[PluginIdentifier("14f02b6a-44d5-4064-9e9d-c04796793ec7", "Ping-pong")]
[Version("1.0.0")]
public class Echo : BasicPlugin
{
    [CommandHandler("echo", Authority = MessageAuthority.Root)]
    public IResponse EchoHandler([Argument] string content, MessageContext context)
    {
        return Reply(context.CommandLineResult.SimpleArgument.ToString());
    }
}