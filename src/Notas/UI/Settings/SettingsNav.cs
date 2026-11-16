using Notas.Theme;

namespace Notas.UI.Settings;

/// <summary>Menu lateral das configurações: título e uma entrada por página.</summary>
internal sealed class SettingsNav : Control
{
    private readonly IReadOnlyList<SettingsPage> _pages;
    private int _selected;
    private int _hover = -1;

    public SettingsNav(IReadOnlyList<SettingsPage> pages)
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        _pages = pages;
        Dock = DockStyle.Left;
        Width = this.Px(208);
    }

    public event Action<int>? Selected;

    private Rectangle Item(int i) => new(this.Px(10), this.Px(60) + i * this.Px(36), Width - this.Px(20), this.Px(34));

    private int IndexAt(Point p) => Enumerable.Range(0, _pages.Count).FirstOrDefault(i => Item(i).Contains(p), -1);

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        int index = IndexAt(e.Location);
        Cursor = index >= 0 ? Cursors.Hand : Cursors.Default;
        if (index != _hover) { _hover = index; Invalidate(); }
    }

    protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover = -1; Invalidate(); }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        int index = IndexAt(e.Location);
        if (index < 0 || index == _selected)
            return;
        _selected = index;
        Invalidate();
        Selected?.Invoke(index);
    }

}
