using Littera.Editor;
using Littera.Theme;

namespace Littera.UI;

internal sealed partial class OpenFilesPanel
{
    /// <summary>Clique ativa; alfinete fixa ou desafixa; × e botão do meio fecham (fixada não fecha por eles); direito abre o menu.</summary>
    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (_sizer.MouseUp(e) || EndDrag())
            return;
        if (e.Button == MouseButtons.Left && NewButtonBounds.Contains(e.Location))
        {
            NewRequested?.Invoke();
            return;
        }

        if (SlotAt(e.Location) is not { } slot)
            return;
        OpenDocument document = slot.Document;
        TabButton button = ButtonAt(slot, e.Location);
        if (e.Button == MouseButtons.Left && button == TabButton.Pin)
        {
            HideTip();
            TogglePin(document);
        }
        else if ((e.Button == MouseButtons.Left && button == TabButton.Close) || (e.Button == MouseButtons.Middle && !document.Pinned))
            CloseRequested?.Invoke(document);
        else if (e.Button == MouseButtons.Left)
            _documents.Activate(document);
        else if (e.Button == MouseButtons.Right)
            MenuRequested?.Invoke(document, PointToScreen(e.Location));
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        int step = e.Delta / 120 * this.Px(ItemStep);
        if (PinnedArea.Contains(e.Location))
            _pinnedScroll -= step;
        else
            _openScroll -= step;
        ClampScroll();
        Invalidate();
    }

    private TabButton ButtonAt(TabSlot slot, Point p) =>
        PinBounds(slot).Contains(p) ? TabButton.Pin
        : !slot.Pinned && CloseBounds(slot.Bounds).Contains(p) ? TabButton.Close : TabButton.None;
}
