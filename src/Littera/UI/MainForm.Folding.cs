using Littera.Editor;
using Littera.Editor.Folding;
using Littera.Editor.Highlights;
using Littera.Editor.Languages;
using ScintillaNET;

namespace Littera.UI;

internal sealed partial class MainForm
{
    private bool _foldsStale = true;
    private Language? _foldsLanguage;

    /// <summary>Trechos recolhíveis (SQL e texto calculados aqui; as outras linguagens vêm da Lexilla) e resumos.</summary>
    private void RefreshFolds(Scintilla editor, OpenDocument document, bool small, Func<string> text)
    {
        bool sql = document.Language is SqlLanguage;
        bool stale = _foldsStale || _foldsLanguage != document.Language;
        _foldsStale = false;
        _foldsLanguage = document.Language;
        if (stale && small && !document.Language.LexerFolds)
            _ = ComputeFoldsAsync(editor, sql, text(), _blocks is not null && _blocksLanguage == document.Language ? _blocks : null);
        else if (small && document.Language.LexerFolds && document.PendingFolds is not null)
        {
            editor.Colorize(0, editor.TextLength); // a Lexilla só calcula os trechos do texto que já coloriu
            RestoreFolds(editor, document);
        }
        else if (!small)
            document.PendingFolds = null; // arquivo grande demais: sem trechos recolhíveis
        _status.SetFolded(EditorFolding.Refresh(editor, sql, CurrentBlock(editor), force: stale));
    }

    /// <summary>Depois de recolher ou expandir: resumos e contagem na barra de status.</summary>
    private void RefreshFoldSummaries(Scintilla editor) =>
        _status.SetFolded(EditorFolding.Refresh(editor, _documents.Active?.Language is SqlLanguage, CurrentBlock(editor), force: true));

    private LineBlock? CurrentBlock(Scintilla editor)
    {
        int line = editor.CurrentLine;
        int index = _blocks?.FindIndex(b => b.Contains(line)) ?? -1;
        return index >= 0 ? _blocks![index] : null;
    }

    /// <summary>Clique na seta da calha.</summary>
    private void OnFoldMarginClick(Scintilla editor, MarginClickEventArgs e)
    {
        int line = editor.LineFromPosition(e.Position);
        if (e.Margin != EditorMargins.Fold || !EditorFolding.IsHeader(editor, line))
            return;
        if (editor.Lines[line].Expanded)
            EditorFolding.Collapse(editor, line);
        else
            EditorFolding.Expand(editor, line);
        RefreshFoldSummaries(editor);
    }
}
