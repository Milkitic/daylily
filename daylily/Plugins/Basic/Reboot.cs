using System.ComponentModel;
using MilkiBotFramework;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins.Basic;

[PluginIdentifier("ee3fc222-b05d-403b-b8c2-542a61b72a4d", "远程重启")]
[PluginLifetime(PluginLifetime.Singleton)]
[Author("milkiyf")]
[Version("1.0.0")]
[Description("允许退出程序或强制关闭进程并重启")]
public class Reboot : BasicPlugin
{
    public enum SubCommandType
    {
        C
    }

    private readonly Bot _bot;
    private readonly BotOptions _botOptions;
    private bool _state;
    private CancellationTokenSource? _cts;

    public Reboot(Bot bot, BotOptions botOptions)
    {
        _bot = bot;
        _botOptions = botOptions;
    }

    [CommandHandler("reboot", Authority = MessageAuthority.Root)]
    public async IAsyncEnumerable<IResponse> RebootCommand(MessageContext messageContext,
        [Option("force")] bool force = false,
        [Argument] SubCommandType? subCommand = null)
    {
        if (subCommand == SubCommandType.C)
        {
            _cts?.Cancel();
            _state = false;
            yield break;
        }

        if (_state) yield break;

        _state = true;
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        foreach (var rootAccount in _botOptions.RootAccounts)
        {
            yield return ToPrivate(rootAccount,
                "20秒后即将重启，发送\"/reboot c\"可取消。\r\n" +
                "注意：若守护进程没有正确运行，将无法自动启动");
        }

        var isCanceled = false;
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(20), _cts.Token);
            if (!force) await _bot.StopAsync(FrameworkConstants.ManualExitCode);
        }
        catch (TaskCanceledException)
        {
            isCanceled = true;
        }

        if (!isCanceled) yield break;
        foreach (var rootAccount in _botOptions.RootAccounts)
        {
            yield return ToPrivate(rootAccount, "已取消重启操作。");
        }
    }
}
