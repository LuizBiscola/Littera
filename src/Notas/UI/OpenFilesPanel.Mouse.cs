using Notas.Editor;
using Notas.Theme;

namespace Notas.UI;

internal sealed partial class OpenFilesPanel
{
    private OpenDocument? _hoverDocument;
    private TabButton _hoverButton;
    private bool _hoverNew;

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (UpdateDrag(e))
            return;
        TabSlot? slot = SlotAt(e.Location);
        TabButton button = slot is { } over ? ButtonAt(over, e.Location) : TabButton.None;
        bool overNew = NewButtonBounds.Contains(e.Location);
        if (slot?.Document == _hoverDocument && button == _hoverButton && overNew == _hoverNew)
            return;
        _hoverDocument = slot?.Document;
        _hoverButton = button;
        _hoverNew = overNew;
        ScheduleTip(slot, button);
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hoverDocument = null;
        _hoverButton = TabButton.None;
        _hoverNew = false;
        HideTip();
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left && SlotAt(e.Location) is { } slot && ButtonAt(slot, e.Location) == TabButton.None)
            BeginPress(slot.Document, e.Location);
    }
}
