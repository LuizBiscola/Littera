using System.Runtime.InteropServices;
using Notas.Editor.Changes;

namespace Notas.UI;

internal sealed partial class ChangePreview
{
    private Rectangle _hoverButton;

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        Rectangle hover = CopyButton.Contains(e.Location) ? CopyButton : RevertButton.Contains(e.Location) ? RevertButton : Rectangle.Empty;
        Cursor = hover.IsEmpty ? Cursors.Default : Cursors.Hand;
        if (hover == _hoverButton)
            return;
        _hoverButton = hover;
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.Button != MouseButtons.Left || AnchorLine < 0)
            return;
        if (CopyButton.Contains(e.Location))
            Copy(string.Join(Environment.NewLine, _removed));
        else if (RevertButton.Contains(e.Location))
        {
            DiffHunk hunk = _hunk;
            string[] original = _removed;
            Dismiss();
            RevertRequested?.Invoke(hunk, original);
        }
    }

    private static void Copy(string text)
    {
        if (text.Length == 0)
            return; // trecho só com linhas novas: não há original para copiar
        try
        {
            Clipboard.SetText(text);
        }
        catch (ExternalException)
        {
            // Área de transferência ocupada por outro programa: o usuário tenta de novo.
        }
    }
}
