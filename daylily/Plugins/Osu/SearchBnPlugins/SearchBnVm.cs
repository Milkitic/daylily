using daylily.Plugins.Osu.Data;
using ICSharpCode.AvalonEdit.Highlighting;
using MilkiBotFramework.Data;

namespace daylily.Plugins.Osu.SearchBnPlugins;

public class SearchBnVm : ViewModelBase
{
    private IHighlightingDefinition? _syntaxHighlighting;

    public SearchBnVm(IReadOnlyList<KeyValuePair<OsuUserInfo, string>> details, string keyword)
    {
        Details = details;
        Keyword = keyword;
    }

    public IReadOnlyList<KeyValuePair<OsuUserInfo, string>> Details { get; }
    public string Keyword { get; }

    public IHighlightingDefinition? SyntaxHighlighting
    {
        get => _syntaxHighlighting;
        set => this.RaiseAndSetIfChanged(ref _syntaxHighlighting, value);
    }
}