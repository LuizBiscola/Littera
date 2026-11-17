using Littera.Theme;

namespace Littera.UI;

internal sealed partial class ChangePreview
{
    private const TextFormatFlags Line = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine
        | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis;

    private Rectangle CopyButton => new(Width - this.Px(210), this.Px(8), this.Px(114), this.Px(22));

    private Rectangle RevertButton => new(Width - this.Px(88), this.Px(8), this.Px(80), this.Px(22));

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        int dpi = DeviceDpi;
        g.Clear(Palette.Background);
        using var border = new Pen(Palette.Border);
        g.DrawRectangle(border, 0, 0, Width - 1, Height - 1);
        g.DrawLine(border, 0, this.Px(HeaderHeight), Width, this.Px(HeaderHeight));
        g.DrawLine(border, 0, Height - this.Px(FooterHeight), Width, Height - this.Px(FooterHeight));

        using (var swatch = new SolidBrush(Palette.ChangeUnsaved))
            g.FillRectangle(swatch, this.Px(12), (this.Px(HeaderHeight) - this.Px(12)) / 2, this.Px(3), this.Px(12));
        Font title = UiFonts.Semibold(11.5f, dpi);
        int titleWidth = TextRenderer.MeasureText(g, Title, title, Size.Empty, TextFormatFlags.NoPadding).Width;
        TextRenderer.DrawText(g, Title, title, new Rectangle(this.Px(23), 0, titleWidth, this.Px(HeaderHeight)), Palette.Text, Line);
        TextRenderer.DrawText(g, Subtitle, UiFonts.Get(11.5f, dpi),
            Rectangle.FromLTRB(this.Px(31) + titleWidth, 0, CopyButton.Left - this.Px(8), this.Px(HeaderHeight)), Palette.TextMuted, Line);

        PaintButton(g, CopyButton, Icons.Copy, "Copiar original", _hoverButton == CopyButton ? Palette.ItemHover : Palette.Background, Palette.TextMuted);
        PaintButton(g, RevertButton, Icons.Undo, "Reverter", Palette.RevertBack, Palette.RevertText);
        PaintRows(g, Rectangle.FromLTRB(1, this.Px(HeaderHeight) + 1, Width - 1, Height - this.Px(FooterHeight)));
        PaintFooter(g, dpi);
    }

    private void PaintButton(Graphics g, Rectangle bounds, char glyph, string text, Color back, Color fore)
    {
        Shapes.FillRounded(g, back, bounds, this.Px(5));
        Icons.Draw(g, glyph, new Rectangle(bounds.X + this.Px(8), bounds.Y, this.Px(12), bounds.Height), fore, 10, DeviceDpi);
        TextRenderer.DrawText(g, text, UiFonts.Semibold(11.5f, DeviceDpi),
            Rectangle.FromLTRB(bounds.X + this.Px(25), bounds.Y, bounds.Right, bounds.Bottom), fore, Line);
    }
}
