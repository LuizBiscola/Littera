using Notas.Theme;

namespace Notas.UI;

internal sealed partial class OpenFilesPanel
{
    private int _scroll;
    private int _hoverIndex = -1;
    private bool _hoverClose;
    private bool _hoverNew;

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        int index = IndexAt(e.Location);
        bool overClose = index >= 0 && CloseBounds(ItemBounds(index)).Contains(e.Location);
        bool overNew = NewButtonBounds.Contains(e.Location);
        if (index == _hoverIndex && overClose == _hoverClose && overNew == _hoverNew)
            return;

        _hoverIndex = index;
        _hoverClose = overClose;
        _hoverNew = overNew;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hoverIndex = -1;
        _hoverClose = _hoverNew = false;
        Invalidate();
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        int content = _documents.Documents.Count * this.Px(ItemHeight + ItemGap) + this.Px(4);
        int max = Math.Max(0, content - ListArea.Height);
        _scroll = Math.Clamp(_scroll - e.Delta / 120 * this.Px(ItemHeight + ItemGap), 0, max);
        Invalidate();
    }

    private int IndexAt(Point p)
    {
        if (!ListArea.Contains(p))
            return -1;
        for (int i = 0; i < _documents.Documents.Count; i++)
        {
            if (ItemBounds(i).Contains(p))
                return i;
        }

        return -1;
    }
}
