using System.Diagnostics.CodeAnalysis;
using MilkiBotFramework;
using MilkiBotFramework.Plugining.CommandLine;

namespace daylily;

public class MyCommandLineAnalyzer : CommandLineAnalyzer
{
    public MyCommandLineAnalyzer(BotOptions botOptions) : base(botOptions)
    {
    }

    public override bool TryAnalyze(string input,
        [NotNullWhen(true)] out CommandLineResult? result,
        out CommandLineException? exception)
    {
        var source = input.AsSpan().Trim();
        if (source.Equals("帮助", StringComparison.InvariantCulture))
        {
            input = $"{GetCommandFlag()}help";
        }
        else if (source.Contains("吃啥", StringComparison.InvariantCulture) ||
             source.Contains("吃什么", StringComparison.InvariantCulture))
        {
            input = $"{GetCommandFlag()}what2eat";
        }
        else if (source.Equals("摆酒席", StringComparison.InvariantCulture))
        {
            input = $"{GetCommandFlag()}what2eat10";
        }

        return base.TryAnalyze(input, out result, out exception);
    }
}