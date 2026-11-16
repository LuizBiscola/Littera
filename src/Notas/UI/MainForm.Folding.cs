using Notas.Editor;
using Notas.Editor.Folding;
using Notas.Editor.Highlights;
using Notas.Editor.Languages;
using ScintillaNET;

namespace Notas.UI;

internal sealed partial class MainForm
{
    private bool _foldsStale = true;
    private Language? _foldsLanguage;

    /// <summary>Trechos recolhíveis (SQL e texto calculados aqui; as outras linguagens vêm da Lexilla) e resumos.</summary>
    private void RefreshFolds(Scintilla editor, OpenDocument document, bool small, Func<string> text)
    {
        bool sql = document.Language is SqlLanguage;
        bool stale = _foldsStale || _foldsLanguage != document.Language;
        if (stale && small && !document.Language.LexerFolds)
        {
            List<LineBlock> blocks = _blocks is not null && _blocksLanguage == document.Language ? _blocks
                : sql ? BlockScanner.SqlStatements(text()) : BlockScanner.Paragraphs(text());
            List<FoldRegion> regions = sql ? FoldScanner.Sql(text(), blocks) : FoldScanner.Paragraphs(blocks);
            EditorFolding.ApplyLevels(editor, FoldLevels.From(editor.Lines.Count, regions));
        }

        _foldsStale = false;
        _foldsLanguage = document.Language;
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
