using Notas.Editor;
using Notas.Editor.Changes;
using Notas.Editor.Search;
using Notas.Theme;
using ScintillaNET;

namespace Notas.UI;

internal sealed partial class MainForm
{
    private ChangePreview? _changePreview;

    /// <summary>Criada no primeiro clique numa barra de alteração.</summary>
    private ChangePreview ChangePreview
    {
        get
        {
            if (_changePreview is null)
            {
                _changePreview = new ChangePreview();
                _changePreview.RevertRequested += (hunk, original) => RevertChange(_boundEditor, hunk, original);
                _changePreview.MouseLeave += (_, _) => _previewHide.Start();
                _editorHost.Controls.Add(_changePreview);
                _documents.ActiveChanged += () => _changePreview.Dismiss();
            }

            _changePreview.Shortcut = ShortcutText("proximaAlteracao");
            return _changePreview;
        }
    }

    /// <summary>Barra laranja: compara o trecho com o arquivo salvo e mostra a prévia logo abaixo dele.</summary>
    private void ShowChangePreview(Scintilla editor, int line)
    {
        if (_changePreview is { Visible: true } open && open.AnchorLine == line)
            return;
        if (_documents.Active is not { Path: { } path } || editor.TextLength > HighlightLimit || !ChangeMarkers.IsUnsaved(editor, line)
            || FileSearch.ReadText(path) is not { } saved)
            return;

        string[] before = SplitLines(saved), after = SplitLines(editor.Text);
        List<DiffHunk> hunks = LineDiff.Hunks(before, after) ?? []; // alterações demais: sem prévia
        int index = hunks.FindIndex(h => line >= h.AfterStart && line < h.AfterStart + Math.Max(1, h.AfterCount));
        if (index < 0)
            return;

        DiffHunk hunk = hunks[index];
        int first = Math.Min(hunk.AfterStart, editor.Lines.Count - 1);
        int last = Math.Min(editor.Lines.Count - 1, hunk.AfterStart + Math.Max(1, hunk.AfterCount) - 1);
        int top = editor.PointYFromPosition(editor.Lines[first].Position);
        int bottom = editor.PointYFromPosition(editor.Lines[last].Position) + editor.Lines[last].Height;
        _previewHide.Stop();
        ChangePreview preview = ChangePreview;
        preview.Show(hunk, before, after, line, new Point(this.Px(14), bottom + this.Px(2)));
        if (preview.Bottom > _editorHost.ClientSize.Height)
            preview.Top = Math.Max(0, top - preview.Height - this.Px(2));
    }

    private static string[] SplitLines(string text) => text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
}
