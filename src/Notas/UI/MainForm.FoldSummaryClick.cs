using Notas.Editor.Folding;
using ScintillaNET;

namespace Notas.UI;

internal sealed partial class MainForm
{
    /// <summary>Prévia e clique no resumo, ligados a cada editor novo.</summary>
    private void AttachFoldPreview(Scintilla editor)
    {
        editor.MouseDwellTime = 400;
        editor.DwellStart += (_, e) => OnEditorDwell(editor, e);
        editor.DwellEnd += (_, _) => _previewHide.Start();
        editor.MouseUp += (_, e) => OnEditorMouseUp(editor, e);
        editor.UpdateUI += (_, e) =>
        {
            if ((e.Change & (UpdateChange.VScroll | UpdateChange.HScroll)) != 0)
                _foldPreview?.Dismiss(); // a prévia ficaria apontando para outra linha
        };
    }

    /// <summary>"Clique no resumo para expandir".</summary>
    private void OnEditorMouseUp(Scintilla editor, MouseEventArgs e)
    {
        int line = e.Button == MouseButtons.Left ? SummaryAt(editor, e.Location) : -1;
        if (line >= 0)
            ExpandFromSummary(editor, line);
    }

    private void ExpandFromSummary(Scintilla editor, int header)
    {
        _foldPreview?.Dismiss();
        EditorFolding.Expand(editor, header);
        RefreshFoldSummaries(editor);
    }

    /// <summary>Cabeçalho recolhido cujo resumo está sob o ponto (à direita do fim do texto da linha); -1 se nenhum.</summary>
    private static int SummaryAt(Scintilla editor, Point point)
    {
        int line = editor.LineFromPosition(editor.CharPositionFromPoint(point.X, point.Y));
        if (!EditorFolding.IsHeader(editor, line) || editor.Lines[line].Expanded)
            return -1;
        Line header = editor.Lines[line];
        int top = editor.PointYFromPosition(header.Position);
        bool inside = point.Y >= top && point.Y < top + header.Height && point.X > editor.PointXFromPosition(TextEnd(header));
        return inside ? line : -1;
    }

    private static int TextEnd(Line line) => line.Position + line.Text.TrimEnd('\r', '\n').Length;
}
