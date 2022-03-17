using System.Diagnostics.CodeAnalysis;
using MilkiBotFramework.Plugining.CommandLine;

namespace daylily;

internal sealed class MyCommandLineAnalyzer : CommandLineAnalyzer
{
    public override bool TryAnalyze(string input,
        [NotNullWhen(true)] out CommandLineResult? result,
        out CommandLineException? exception)
    {
        var source = input.AsSpan().Trim();
        if (source.Equals("帮助", StringComparison.InvariantCulture))
        {
            input = $"{CommandFlag}help";
        }

        return base.TryAnalyze(input, out result, out exception);
    }
}