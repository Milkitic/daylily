using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using MilkiBotFramework.Imaging.Wpf;
using Image = SixLabors.ImageSharp.Image;

namespace daylily.Plugins.Osu.SearchBnPlugins;

/// <summary>
/// SearchBnControl.xaml 的交互逻辑
/// </summary>
public partial class SearchBnControl : WpfDrawingControl
{
    private const string XshdTemplate =
        @"<SyntaxDefinition name=""C#"" xmlns=""http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008"">
    <Color name=""kw"" fontWeight=""bold"" background=""#E0E0E0"" foreground=""#F02432"" />
    <RuleSet>
        <Rule color=""kw"">
            {{rep}}
        </Rule>
    </RuleSet>
</SyntaxDefinition>";

    private readonly HashSet<char> _allKeywords = new()
    { '\\', '^', '$', '.', '[', ']', '*', '+', '?', '{', '}', '|', '(', ')', ',' };

    private readonly SearchBnVm _viewModel;

    public SearchBnControl(object viewModel, Image? sourceImage = null) : base(viewModel, sourceImage)
    {
        _viewModel = (SearchBnVm)viewModel;
        InitializeComponent();
    }

    protected override Visual GetDrawingVisual(out Size size)
    {
        size = new Size(StackPanel1.ActualWidth, StackPanel1.ActualHeight);
        return StackPanel1;
    }

    private void SearchBnControl_OnInitialized(object? sender, EventArgs e)
    {
        var xshdTemplate = XshdTemplate;

        var keyWord = _viewModel.Keyword;
        var sb = new StringBuilder();
        foreach (var c in keyWord)
        {
            if (_allKeywords.Contains(c))
                sb.Append('\\');
            sb.Append(c);
        }

        xshdTemplate = xshdTemplate.Replace("{{rep}}", sb.ToString());

        using var stringReader = new StringReader(xshdTemplate);
        using var reader = XmlReader.Create(stringReader);
        var xshd = HighlightingLoader.LoadXshd(reader);
        var xshdRuleSet = (XshdRuleSet)xshd.Elements[1];
        xshdRuleSet.IgnoreCase = true;

        var syntaxHighlighting = HighlightingLoader.Load(xshd, HighlightingManager.Instance);
        _viewModel.SyntaxHighlighting = syntaxHighlighting;
    }

    private async void SearchBnControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        for (int i = 0; i < ItemsControl.Items.Count; i++)
        {
            var uiElement = (ContentPresenter)ItemsControl.ItemContainerGenerator.ContainerFromIndex(i);
            if (uiElement.ContentTemplate.FindName("TextEditor", uiElement) is TextEditor tb)
            {
                tb.Text = _viewModel.Details[i].Value;
                tb.SyntaxHighlighting = _viewModel.SyntaxHighlighting;
            }
        }

        await Task.Delay(16);
        await FinishDrawing();
    }
}