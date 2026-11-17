using Littera.Theme;

namespace Littera.UI.Commands;

internal sealed partial class CommandPalette
{
    private int RowAt(Point p)
    {
        int y = p.Y - this.Px(InputHeight);
        int index = y < 0 ? -1 : _top + y / this.Px(RowHeight);
        return index < _shown.Count && index < _top + MaxRows ? index : -1;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        int index = RowAt(e.Location);
        if (index < 0 || index == _selected)
            return;
        _selected = index;
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.Button == MouseButtons.Left)
            Choose(RowAt(e.Location));
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        _top = Math.Clamp(_top - e.Delta / 120 * 3, 0, Math.Max(0, _shown.Count - MaxRows));
        Invalidate();
    }
}
