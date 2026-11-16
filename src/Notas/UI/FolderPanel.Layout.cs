using Notas.Theme;

namespace Notas.UI;

internal sealed partial class FolderPanel
{
    private const int RowHeight = 28;
    private const int RowStep = 29;

    private int _scroll;

    private Rectangle CloseButton => new(Width - this.Px(34), this.Px(14), this.Px(24), this.Px(24));

    private Rectangle CollapseButton => new(Width - this.Px(62), this.Px(14), this.Px(24), this.Px(24));

    private Rectangle RefreshButton => new(Width - this.Px(90), this.Px(14), this.Px(24), this.Px(24));

    private Rectangle FilterBox => new(this.Px(16), this.Px(92), Width - this.Px(26), this.Px(30));

    private Rectangle TreeArea => Rectangle.FromLTRB(0, this.Px(HeaderHeight), Width, Height - this.Px(FooterHeight));

    private Rectangle RowBounds(int index) =>
        new(this.Px(8), TreeArea.Top + this.Px(2) + index * this.Px(RowStep) - _scroll, Width - this.Px(16), this.Px(RowHeight));

    private int RowAt(Point p)
    {
        if (!TreeArea.Contains(p))
            return -1;
        int index = (p.Y - TreeArea.Top - this.Px(2) + _scroll) / this.Px(RowStep);
        return index >= 0 && index < _rows.Count && RowBounds(index).Contains(p) ? index : -1;
    }

    private void ClampScroll()
    {
        int content = _rows.Count * this.Px(RowStep) + this.Px(4);
        _scroll = Math.Clamp(_scroll, 0, Math.Max(0, content - TreeArea.Height));
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        _scroll -= e.Delta / 120 * 3 * this.Px(RowStep);
        ClampScroll();
        Invalidate();
    }

    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        _filter.Font = UiFonts.Get(12, DeviceDpi);
        int height = _filter.PreferredHeight;
        Rectangle box = FilterBox;
        _filter.SetBounds(box.X + this.Px(31), box.Y + (box.Height - height) / 2, box.Width - this.Px(39), height);
        ClampScroll();
    }
}
