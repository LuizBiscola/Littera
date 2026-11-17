using System.Runtime.InteropServices;
using Littera.Editor.Folding;

namespace Littera.UI;

internal sealed partial class FoldPreview
{
    private Rectangle _hoverButton;

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        Rectangle hover = CopyButton.Contains(e.Location) ? CopyButton : ExpandButton.Contains(e.Location) ? ExpandButton : Rectangle.Empty;
        Cursor = hover.IsEmpty ? Cursors.Default : Cursors.Hand;
        if (hover == _hoverButton)
            return;
        _hoverButton = hover;
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.Button != MouseButtons.Left || _editor is not { } editor || Header < 0)
            return;
        if (CopyButton.Contains(e.Location))
            Copy(string.Join(Environment.NewLine, EditorFolding.HiddenLines(editor, Header)));
        else if (ExpandButton.Contains(e.Location))
        {
            int header = Header;
            Dismiss();
            ExpandRequested?.Invoke(editor, header);
        }
    }

    private static void Copy(string text)
    {
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
