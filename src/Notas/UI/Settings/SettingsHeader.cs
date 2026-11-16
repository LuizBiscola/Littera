using Notas.Theme;

namespace Notas.UI.Settings;

/// <summary>Título e subtítulo da página aberta, com o X de fechar à direita.</summary>
internal sealed class SettingsHeader : Control
{
    private readonly Action _close;
    private SettingsPage? _page;
    private bool _closeHot;

    public SettingsHeader(Action close)
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        _close = close;
        Dock = DockStyle.Top;
        Height = this.Px(80);
    }

    public Rectangle CloseBounds => new(Width - this.Px(44), this.Px(18), this.Px(28), this.Px(28));

    public void SetPage(SettingsPage page)
    {
        _page = page;
        Invalidate();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        bool hot = CloseBounds.Contains(e.Location);
        if (hot != _closeHot) { _closeHot = hot; Invalidate(); }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (CloseBounds.Contains(e.Location))
            _close();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.Clear(Palette.Background);
        if (_page is null)
            return;
        TextRenderer.DrawText(g, _page.Name, UiFonts.Semibold(17, DeviceDpi), new Point(this.Px(28), this.Px(20)), Palette.Text, TextFormatFlags.NoPadding);
        TextRenderer.DrawText(g, _page.Subtitle, UiFonts.Get(12.5f, DeviceDpi), new Point(this.Px(28), this.Px(48)), Palette.TextMuted, TextFormatFlags.NoPadding);
        if (_closeHot)
            Shapes.FillRounded(g, Palette.ItemHover, CloseBounds, this.Px(5));
        Icons.Draw(g, Icons.Close, CloseBounds, Palette.TextMuted, 10, DeviceDpi);
    }
}
