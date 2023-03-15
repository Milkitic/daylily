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

        return base.TryAnalyze(input, out result, out exception);
    }
}