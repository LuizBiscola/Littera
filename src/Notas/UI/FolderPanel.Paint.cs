using Notas.Theme;

namespace Notas.UI;

internal sealed partial class FolderPanel
{
    private const TextFormatFlags Line = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine
        | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding;

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        int dpi = DeviceDpi;
        g.Clear(Palette.Panel);
        using (var border = new Pen(Palette.Border))
            g.DrawLine(border, 0, 0, 0, Height);

        PaintHeader(g, dpi);
        g.SetClip(TreeArea);
        for (int i = 0; i < _rows.Count; i++)
            PaintRow(g, i, dpi);
        g.ResetClip();

        string footer = Filtering ? $"{_rows.Count} {Plural(_rows.Count, "encontrado", "encontrados")}" : _countText;
        TextRenderer.DrawText(g, footer, UiFonts.Get(10.5f, dpi),
            new Rectangle(this.Px(16), Height - this.Px(FooterHeight), Width - this.Px(32), this.Px(FooterHeight)), Palette.TextFaint, Line);
    }

    private void PaintHeader(Graphics g, int dpi)
    {
        TextRenderer.DrawText(g, "PASTA", UiFonts.Semibold(10.5f, dpi), new Point(this.Px(16), this.Px(20)), Palette.TextMuted,
            TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
        PaintButton(g, RefreshButton, Icons.Refresh, dpi);
        PaintButton(g, CollapseButton, Icons.CollapseAll, dpi);
        PaintButton(g, CloseButton, Icons.Close, dpi);

        int textWidth = Width - this.Px(26);
        TextRenderer.DrawText(g, Path.GetFileName(Folder) is { Length: > 0 } name ? name : Folder, UiFonts.Semibold(14, dpi),
            new Rectangle(this.Px(16), this.Px(46), textWidth, this.Px(20)), Palette.Text, Line);
        TextRenderer.DrawText(g, Folder, UiFonts.Get(11, dpi),
            new Rectangle(this.Px(16), this.Px(66), textWidth, this.Px(16)), Palette.TextFaint, Line | TextFormatFlags.PathEllipsis);

        Rectangle box = FilterBox;
        Shapes.FillRounded(g, _filter.Focused ? Palette.Accent : Palette.Border, box, this.Px(6));
        box.Inflate(-1, -1);
        Shapes.FillRounded(g, Palette.Background, box, this.Px(5));
        Icons.Draw(g, Icons.Search, new Rectangle(box.X + this.Px(9), box.Y + this.Px(8), this.Px(13), this.Px(13)), Palette.TextFaint, 11, dpi);
    }

    private void PaintButton(Graphics g, Rectangle bounds, char glyph, int dpi)
    {
        if (bounds == _hoverButton)
            Shapes.FillRounded(g, Palette.ItemActive, bounds, this.Px(5));
        Icons.Draw(g, glyph, bounds, Palette.TextMuted, 12, dpi);
    }
}
