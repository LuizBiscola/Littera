using Littera.Editor.Highlights;
using Littera.Editor.Languages;
using ScintillaNET;

namespace Littera.UI;

internal sealed partial class MainForm
{
    /// <summary>Acima disso (arquivos enormes) os destaques ficam desligados: varrer tudo a cada parada do cursor não compensa.</summary>
    private const int HighlightLimit = 2_000_000;

    private readonly System.Windows.Forms.Timer _highlightTimer = new() { Interval = 120 };
    private List<LineBlock>? _blocks;
    private Language? _blocksLanguage;

    /// <summary>Espera o cursor parar para não recalcular a cada tecla.</summary>
    private void ScheduleHighlights(bool textChanged)
    {
        if (textChanged)
        {
            _blocks = null;
            _foldsStale = true;
            _foldVersion++;
            InvalidateChanges();
        }

        _highlightTimer.Stop();
        _highlightTimer.Start();
    }

    private void RefreshHighlights()
    {
        _highlightTimer.Stop();
        if (_boundEditor is not { } editor || _documents.Active is not { IsLoading: false } document)
            return;

        bool small = editor.TextLength <= HighlightLimit;
        string? text = null;
        string Text() => text ??= editor.Text; // copia o texto uma vez só, para blocos e dobras
        string? block = null;
        if (small && _settings.Editor.HighlightCurrentBlock)
        {
            bool sql = document.Language is SqlLanguage;
            if (_blocksLanguage != document.Language)
                _blocks = null;
            _blocks ??= sql ? BlockScanner.SqlStatements(Text()) : BlockScanner.Paragraphs(Text());
            _blocksLanguage = document.Language;
            block = BlockHighlighter.Update(editor, _blocks, sql ? "Instrução" : "Parágrafo");
        }
        else
        {
            BlockHighlighter.Clear(editor);
        }

        string? occurrences = small && _settings.Editor.HighlightOccurrences ? OccurrenceHighlighter.Update(editor) : null;
        if (occurrences is null)
            OccurrenceHighlighter.Clear(editor);
        _status.SetBlock(block);
        _status.SetOccurrences(occurrences);
        RefreshFolds(editor, document, small, Text);
        RefreshChanges(editor, document, small, Text);
    }
}
