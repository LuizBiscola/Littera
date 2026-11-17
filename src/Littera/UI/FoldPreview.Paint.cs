using Littera.Theme;

namespace Littera.UI;

internal sealed partial class FoldPreview
{
    private const TextFormatFlags Line = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine
        | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis;

    private Rectangle CopyButton => new(Width - this.Px(166), this.Px(8), this.Px(70), this.Px(22));

    private Rectangle ExpandButton => new(Width - this.Px(90), this.Px(8), this.Px(82), this.Px(22));

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        int dpi = DeviceDpi;
        g.Clear(Palette.Background);
        using var border = new Pen(Palette.Border);
        g.DrawRectangle(border, 0, 0, Width - 1, Height - 1);
        g.DrawLine(border, 0, this.Px(HeaderHeight), Width, this.Px(HeaderHeight));
        g.DrawLine(border, 0, Height - this.Px(FooterHeight), Width, Height - this.Px(FooterHeight));

        Icons.Draw(g, Icons.View, new Rectangle(this.Px(12), 0, this.Px(13), this.Px(HeaderHeight)), Palette.TextFaint, 11, dpi);
        Font title = UiFonts.Semibold(11.5f, dpi);
        int titleWidth = TextRenderer.MeasureText(g, _title, title, Size.Empty, TextFormatFlags.NoPadding).Width;
        TextRenderer.DrawText(g, _title, title, new Rectangle(this.Px(31), 0, titleWidth, this.Px(HeaderHeight)), Palette.Text, Line);
        TextRenderer.DrawText(g, _subtitle, UiFonts.Get(11.5f, dpi),
            Rectangle.FromLTRB(this.Px(37) + titleWidth, 0, CopyButton.Left - this.Px(8), this.Px(HeaderHeight)), Palette.TextMuted, Line);
        PaintButton(g, CopyButton, Icons.Copy, "Copiar", _hoverButton == CopyButton ? Palette.ItemHover : Palette.Background, Palette.TextMuted);
        PaintButton(g, ExpandButton, Icons.ChevronDown, "Expandir", Palette.AccentSoft, Palette.Accent);

        PaintCode(g, Rectangle.FromLTRB(1, this.Px(HeaderHeight) + 1, Width - 1, Height - this.Px(FooterHeight)));
        PaintFooter(g, dpi);
    }

    private void PaintButton(Graphics g, Rectangle bounds, char glyph, string text, Color back, Color fore)
    {
        Shapes.FillRounded(g, back, bounds, this.Px(5));
        Icons.Draw(g, glyph, new Rectangle(bounds.X + this.Px(8), bounds.Y, this.Px(12), bounds.Height), fore, 10, DeviceDpi);
        TextRenderer.DrawText(g, text, UiFonts.Semibold(11.5f, DeviceDpi),
            Rectangle.FromLTRB(bounds.X + this.Px(25), bounds.Y, bounds.Right, bounds.Bottom), fore, Line);
    }

    private void PaintFooter(Graphics g, int dpi)
    {
        var footer = Rectangle.FromLTRB(this.Px(12), Height - this.Px(FooterHeight), Width - this.Px(12), Height);
        TextRenderer.DrawText(g, "Clique no resumo para expandir", UiFonts.Get(11, dpi), footer, Palette.TextFaint, Line);
        if (string.IsNullOrEmpty(Shortcut))
            return;

        Font keys = UiFonts.Get(10.5f, dpi);
        int width = TextRenderer.MeasureText(g, Shortcut, keys, Size.Empty, TextFormatFlags.NoPadding).Width + this.Px(10);
        var cap = new Rectangle(footer.Right - width, footer.Y + (footer.Height - this.Px(16)) / 2, width, this.Px(16));
        Shapes.FillRounded(g, Palette.Border, cap, this.Px(3));
        cap.Inflate(-1, -1);
        Shapes.FillRounded(g, Palette.Background, cap, this.Px(3));
        TextRenderer.DrawText(g, Shortcut, keys, cap, Palette.TextMuted, Line | TextFormatFlags.HorizontalCenter);
    }
}
