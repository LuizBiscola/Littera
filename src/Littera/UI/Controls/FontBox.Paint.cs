using Littera.Theme;

namespace Littera.UI.Controls;

internal sealed partial class FontBox
{
    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.Clear(Parent?.BackColor ?? Palette.Background);
        Shapes.FillRounded(g, _text.Focused ? Palette.Accent : Palette.Border, new Rectangle(0, 0, Width, Height), this.Px(6));
        Shapes.FillRounded(g, Palette.Background, new Rectangle(1, 1, Width - 2, Height - 2), this.Px(5));
        Icons.Draw(g, Icons.ChevronDownSmall, ArrowBounds, Palette.TextMuted, 10, DeviceDpi);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (ArrowBounds.Contains(e.Location))
            ShowList();
        else
            _text.Focus();
    }
}
