using Notas.Editor.Editing;
using ScintillaNET;

namespace Notas.UI;

internal sealed partial class MainForm
{
    /// <summary>Atalhos de edição só valem com o foco no editor (Ctrl+D na caixa de busca não duplica linha).</summary>
    private void InEditor(Action<Scintilla> edit)
    {
        if (_boundEditor is { Focused: true } editor)
            edit(editor);
    }

    /// <summary>Ctrl+/: comenta ou descomenta as linhas da seleção, mantendo a seleção no mesmo texto.</summary>
    private void ToggleLineComment(Scintilla editor)
    {
        if (_documents.Active?.Language.LineComment is not { } prefix)
            return;
        int first = editor.LineFromPosition(editor.SelectionStart), last = editor.LineFromPosition(editor.SelectionEnd);
        if (last > first && editor.SelectionEnd == editor.Lines[last].Position)
            last--; // seleção que termina no começo de uma linha não inclui essa linha
        string[] lines = [.. Enumerable.Range(first, last - first + 1).Select(l => editor.Lines[l].Text.TrimEnd('\r', '\n'))];
        var edits = LineComments.Toggle(lines, prefix).Select(e => (At: editor.Lines[first + e.Line].Position + e.Column, Edit: e)).ToList();
        if (edits.Count == 0)
            return;

        int caret = Shift(editor.CurrentPosition, edits), anchor = Shift(editor.AnchorPosition, edits);
        editor.BeginUndoAction();
        foreach ((int at, LineEdit edit) in Enumerable.Reverse(edits)) // de baixo para cima: as posições de cima não mudam
        {
            if (edit.Remove > 0)
                editor.DeleteRange(at, edit.Remove);
            if (edit.Insert.Length > 0)
                editor.InsertText(at, edit.Insert);
        }

        editor.EndUndoAction();
        editor.SetSelection(caret, anchor);
    }

    /// <summary>Onde uma posição vai parar depois das alterações (todas medidas no texto original).</summary>
    private static int Shift(int position, List<(int At, LineEdit Edit)> edits)
    {
        int shifted = position;
        foreach ((int at, LineEdit edit) in edits)
        {
            if (position > at)
                shifted -= Math.Min(edit.Remove, position - at);
            if (position >= at)
                shifted += edit.Insert.Length;
        }

        return shifted;
    }
}
