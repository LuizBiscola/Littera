using Notas.Editor;
using Notas.Editor.Languages;
using Notas.Services;

namespace Notas.UI;

internal sealed partial class MainForm
{
    private static readonly string[] IndentChoices = ["Espaços: 2", "Espaços: 4", "Tabulação"];

    private void OnStatusSegmentClicked(StatusSegment segment, Rectangle bounds)
    {
        if (_documents.Active is not { IsLoading: false } document)
            return;

        ContextMenuStrip? menu = segment switch
        {
            StatusSegment.Encoding => Menus.Choices(this, "Salvar com o encoding", Enum.GetValues<TextEncodingKind>(),
                document.Format.Encoding, k => k == TextEncodingKind.Ansi ? "ANSI (Windows-1252)" : k.Label(),
                k => _documents.SetEncoding(document, k)),
            StatusSegment.LineEnding => Menus.Choices(this, "Converter quebras de linha", Enum.GetValues<LineEnding>(),
                document.Format.LineEnding, e => e.Label(), e => _documents.SetLineEnding(document, e)),
            StatusSegment.Indent => Menus.Choices(this, "Indentação", IndentChoices,
                _settings.Editor.UseTabs ? IndentChoices[2] : $"Espaços: {_settings.Editor.IndentSize}", c => c, ApplyIndent),
            StatusSegment.Language => Menus.Choices(this, "Linguagem", LanguageRegistry.All,
                document.Language, l => l.Name, l => _documents.SetLanguage(document, l)),
            _ => null,
        };
        menu?.Show(bounds.Location, ToolStripDropDownDirection.AboveRight);
    }

    private void ApplyIndent(string choice)
    {
        bool tabs = choice == IndentChoices[2];
        int size = choice == IndentChoices[0] ? 2 : 4;
        _settings.Editor.UseTabs = tabs;
        _settings.Editor.IndentSize = size;
        foreach (OpenDocument document in _documents.Documents)
        {
            if (document.Editor is not { } editor)
                continue;
            editor.UseTabs = tabs;
            editor.IndentWidth = size;
            editor.TabWidth = size;
        }

        UpdateStatus();
    }
}
