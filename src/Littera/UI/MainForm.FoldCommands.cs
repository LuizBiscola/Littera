using Littera.Editor.Folding;
using ScintillaNET;

namespace Littera.UI;

internal sealed partial class MainForm
{
    /// <summary>Ctrl+Shift+[: recolhe o trecho do cursor (o da própria linha ou o que a contém).</summary>
    private void CollapseCurrentFold()
    {
        if (_boundEditor is not { } editor)
            return;
        int line = editor.CurrentLine;
        int header = EditorFolding.IsHeader(editor, line) && editor.Lines[line].Expanded ? line : editor.Lines[line].FoldParent;
        if (header < 0)
            return;
        EditorFolding.Collapse(editor, header);
        RefreshFoldSummaries(editor);
    }

    /// <summary>Ctrl+Shift+]: expande o trecho recolhido na linha do cursor.</summary>
    private void ExpandCurrentFold()
    {
        if (_boundEditor is not { } editor)
            return;
        int line = editor.CurrentLine;
        if (!EditorFolding.IsHeader(editor, line) || editor.Lines[line].Expanded)
            return;
        EditorFolding.Expand(editor, line);
        RefreshFoldSummaries(editor);
    }

    /// <summary>Recolher ou expandir tudo (menu e "Expandir tudo" da barra de status).</summary>
    private void SetAllFolds(bool expand)
    {
        if (_boundEditor is not { } editor)
            return;
        editor.FoldAll(expand ? FoldAction.Expand : FoldAction.Contract);
        int line = editor.CurrentLine;
        while (line > 0 && !editor.Lines[line].Visible)
            line = editor.Lines[line].FoldParent; // o cursor sobe para o cabeçalho que ficou à mostra
        if (line != editor.CurrentLine)
            editor.GotoPosition(editor.Lines[line].Position);
        RefreshFoldSummaries(editor);
    }
}
