using Notas.Editor;
using Notas.Editor.Changes;
using ScintillaNET;

namespace Notas.UI;

internal sealed partial class MainForm
{
    /// <summary>Clique na barra de alteração ou mouse parado sobre ela abre a prévia; rolar ou editar fecha.</summary>
    private void AttachChangePreview(Scintilla editor)
    {
        editor.MarginClick += (_, e) =>
        {
            if (e.Margin == EditorMargins.Changes)
                ShowChangePreview(editor, editor.LineFromPosition(e.Position));
        };
        editor.DwellStart += (_, e) =>
        {
            if (ChangeLineAt(editor, new Point(e.X, e.Y)) is int line)
                ShowChangePreview(editor, line);
        };
        editor.MouseMove += (_, e) => SetChangeHover(editor, ChangeLineAt(editor, e.Location) is int line ? _changeSet?.HunkAt(line, editor.Lines.Count) : null);
        editor.MouseLeave += (_, _) => SetChangeHover(editor, null);
        if (editor is NotasEditor notas)
            notas.HandCursorAt = point => ChangeLineAt(editor, point) is not null;
        editor.UpdateUI += (_, e) =>
        {
            if ((e.Change & (UpdateChange.VScroll | UpdateChange.HScroll)) != 0)
                _changePreview?.Dismiss();
        };
        editor.Insert += (_, _) => _changePreview?.Dismiss(); // o diff mostrado ficaria velho
        editor.Delete += (_, _) => _changePreview?.Dismiss();
    }

    /// <summary>"Reverter": as linhas da versão salva voltam no lugar do trecho, num único desfazer.</summary>
    private static void RevertChange(Scintilla? editor, DiffHunk hunk, string[] original)
    {
        if (editor is null)
            return;
        string eol = editor.EolMode switch { Eol.Lf => "\n", Eol.Cr => "\r", _ => "\r\n" };
        int start = hunk.AfterStart < editor.Lines.Count ? editor.Lines[hunk.AfterStart].Position : editor.TextLength;
        int end = hunk.AfterCount > 0 ? editor.Lines[hunk.AfterStart + hunk.AfterCount - 1].EndPosition : start;
        string text = string.Concat(original.Select(line => line + eol));
        bool lastLineWithoutEol = end == editor.TextLength && end > start && editor.GetCharAt(end - 1) is not ('\n' or '\r');
        if (lastLineWithoutEol && text.EndsWith(eol, StringComparison.Ordinal))
            text = text[..^eol.Length]; // trecho no fim de um arquivo sem quebra final
        editor.SetTargetRange(start, end);
        editor.ReplaceTarget(text);
    }
}
