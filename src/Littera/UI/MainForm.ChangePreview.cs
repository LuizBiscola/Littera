using Littera.Editor;
using Littera.Editor.Changes;
using Littera.Theme;
using ScintillaNET;

namespace Littera.UI;

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

    /// <summary>Barra laranja: mostra o trecho da última comparação com o disco logo abaixo dele.</summary>
    private void ShowChangePreview(Scintilla editor, int line)
    {
        if (_changePreview is { Visible: true } open && open.AnchorLine == line)
            return;
        if (_changeSet?.HunkAt(line, editor.Lines.Count) is not { } hunk || _documents.Active is not { } document
            || (document.Baseline ?? (document.Path is null ? Untitled : null)) is not { } baseline)
            return;

        string[] before = baseline.Saved, after = ChangeBaseline.Lines(editor.Text);
        if (hunk.BeforeStart + hunk.BeforeCount > before.Length || hunk.AfterStart + hunk.AfterCount > after.Length)
            return; // comparação de um texto que já mudou: a próxima chega em instantes

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
}
