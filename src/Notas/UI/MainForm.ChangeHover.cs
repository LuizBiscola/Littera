using Notas.Editor;
using Notas.Editor.Changes;
using ScintillaNET;

namespace Notas.UI;

internal sealed partial class MainForm
{
    private DiffHunk? _hoverHunk;

    /// <summary>Linha de um trecho laranja sob o ponto, se ele estiver na coluna das barras; null fora dela.</summary>
    private int? ChangeLineAt(Scintilla editor, Point point)
    {
        if (editor != _boundEditor || _changeSet is not { } changes || point.X < 0 || point.X >= editor.Margins[EditorMargins.Changes].Width)
            return null;
        int line = editor.LineFromPosition(editor.CharPositionFromPoint(point.X, point.Y));
        Line target = editor.Lines[line];
        int top = editor.PointYFromPosition(target.Position);
        bool inside = point.Y >= top && point.Y < top + target.Height;
        return inside && changes.HunkAt(line, editor.Lines.Count) is not null ? line : null;
    }

    /// <summary>Mouse sobre um trecho laranja: a barra dele engrossa, sinal de que dá para clicar.</summary>
    private void SetChangeHover(Scintilla editor, DiffHunk? hunk)
    {
        if (Nullable.Equals(hunk, _hoverHunk))
            return;
        _hoverHunk = hunk;
        ChangeMarkers.SetHover(editor, hunk);
    }
}
