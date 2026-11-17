using Notas.Editor.Folding;
using Notas.Editor.Highlights;
using ScintillaNET;

namespace Notas.UI;

internal sealed partial class MainForm
{
    private int _foldVersion; // muda a cada edição: um cálculo que ficou para trás é descartado

    /// <summary>
    /// Num arquivo de 2 milhões de caracteres a varredura leva ~70 ms: roda fora da thread da interface
    /// e os níveis só são aplicados se o texto não mudou enquanto isso.
    /// </summary>
    private async Task ComputeFoldsAsync(Scintilla editor, bool sql, string content, List<LineBlock>? blocks)
    {
        int version = _foldVersion, lines = editor.Lines.Count;
        int[] levels = await Task.Run(() =>
        {
            List<LineBlock> scanned = blocks ?? (sql ? BlockScanner.SqlStatements(content) : BlockScanner.Paragraphs(content));
            return FoldLevels.From(lines, sql ? FoldScanner.Sql(content, scanned) : FoldScanner.Paragraphs(scanned));
        });
        if (version != _foldVersion || _boundEditor != editor)
            return;

        EditorFolding.ApplyLevels(editor, levels);
        if (_documents.Active is { } document && document.Editor == editor)
            RestoreFolds(editor, document); // sessão anterior: só agora os cabeçalhos existem
        RefreshFoldSummaries(editor);
    }
}
