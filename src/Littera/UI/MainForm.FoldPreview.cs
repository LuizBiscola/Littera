using Littera.Editor.Folding;
using Littera.Theme;
using ScintillaNET;

namespace Littera.UI;

internal sealed partial class MainForm
{
    private readonly System.Windows.Forms.Timer _previewHide = new() { Interval = 250 };
    private FoldPreview? _foldPreview;

    /// <summary>Criada na primeira vez que o mouse para sobre um resumo.</summary>
    private FoldPreview FoldPreview
    {
        get
        {
            if (_foldPreview is null)
            {
                _foldPreview = new FoldPreview();
                _foldPreview.ExpandRequested += ExpandFromSummary;
                _foldPreview.MouseLeave += (_, _) => _previewHide.Start();
                _editorHost.Controls.Add(_foldPreview);
                _previewHide.Tick += (_, _) => HidePreviewIfAway();
                _documents.ActiveChanged += () => _foldPreview.Dismiss();
            }

            _foldPreview.Shortcut = ShortcutText("expandirBloco");
            return _foldPreview;
        }
    }

    /// <summary>Mouse parado sobre um resumo: mostra a prévia logo abaixo da linha (ou acima, sem espaço).</summary>
    private void OnEditorDwell(Scintilla editor, DwellEventArgs e)
    {
        int line = SummaryAt(editor, new Point(e.X, e.Y));
        if (line < 0 || (_foldPreview is { Visible: true } shown && shown.Header == line))
            return;

        _previewHide.Stop();
        Line header = editor.Lines[line];
        int top = editor.PointYFromPosition(header.Position);
        int x = Math.Clamp(editor.PointXFromPosition(TextEnd(header)) + this.Px(12), 0, Math.Max(0, editor.Width - this.Px(516)));
        FoldPreview preview = FoldPreview;
        preview.Show(editor, line, new Point(x, top + header.Height + this.Px(2)), above: false);
        if (preview.Bottom > _editorHost.ClientSize.Height)
            preview.Top = Math.Max(0, top - preview.Height - this.Px(2));
    }

    /// <summary>Some quando o mouse não está nem na prévia nem no resumo que a abriu.</summary>
    private void HidePreviewIfAway()
    {
        _previewHide.Stop();
        if (_changePreview is { Visible: true } change && !change.ClientRectangle.Contains(change.PointToClient(Cursor.Position))
            && !(_boundEditor is { } current && current.PointToClient(Cursor.Position).X < current.Margins[Editor.EditorMargins.Changes].Width))
            change.Dismiss(); // a prévia da alteração some quando o mouse sai dela
        if (_foldPreview is not { Visible: true } preview || preview.ClientRectangle.Contains(preview.PointToClient(Cursor.Position)))
            return;
        if (_boundEditor is { } editor && SummaryAt(editor, editor.PointToClient(Cursor.Position)) == preview.Header)
            return;
        preview.Dismiss();
    }
}
