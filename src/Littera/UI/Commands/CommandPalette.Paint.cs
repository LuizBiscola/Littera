using Littera.Theme;

namespace Littera.UI.Commands;

internal sealed partial class CommandPalette
{
    private const TextFormatFlags Line = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine
        | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis;

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.Clear(Palette.Background);
        using (var border = new Pen(Palette.Border))
            g.DrawRectangle(border, 0, 0, Width - 1, Height - 1);

        var box = new Rectangle(this.Px(8), this.Px(8), Width - this.Px(16), this.Px(InputHeight - 16));
        Shapes.FillRounded(g, Palette.Accent, box, this.Px(6));
        box.Inflate(-1, -1);
        Shapes.FillRounded(g, Palette.Background, box, this.Px(5));

        Font font = UiFonts.Get(13, DeviceDpi);
        if (_shown.Count == 0)
        {
            TextRenderer.DrawText(g, "Nenhum comando encontrado", font, RowBounds(0), Palette.TextFaint, Line | TextFormatFlags.HorizontalCenter);
            return;
        }

        for (int i = _top; i < Math.Min(_shown.Count, _top + MaxRows); i++)
            PaintRow(g, _shown[i], RowBounds(i - _top), i == _selected, font);
    }

    private Rectangle RowBounds(int row) =>
        new(this.Px(6), this.Px(InputHeight) + row * this.Px(RowHeight), Width - this.Px(12), this.Px(RowHeight));

    private void PaintRow(Graphics g, PaletteItem item, Rectangle row, bool selected, Font font)
    {
        if (selected)
            Shapes.FillRounded(g, Palette.ItemActive, row, this.Px(5));

        int right = row.Right - this.Px(10);
        if (item.Shortcut is { Length: > 0 } keys)
        {
            Font small = UiFonts.Get(11, DeviceDpi);
            int width = TextRenderer.MeasureText(g, keys, small, Size.Empty, TextFormatFlags.NoPadding).Width + this.Px(12);
            var cap = new Rectangle(right - width, row.Y + (row.Height - this.Px(20)) / 2, width, this.Px(20));
            Shapes.FillRounded(g, Palette.ItemHover, cap, this.Px(4));
            TextRenderer.DrawText(g, keys, small, cap, Palette.TextSecondary, Line | TextFormatFlags.HorizontalCenter);
            right = cap.Left - this.Px(12);
        }

        int labelWidth = TextRenderer.MeasureText(g, item.Label, font, Size.Empty, TextFormatFlags.NoPadding).Width;
        var label = Rectangle.FromLTRB(row.X + this.Px(12), row.Y, Math.Min(right, row.X + this.Px(12) + labelWidth), row.Bottom);
        TextRenderer.DrawText(g, item.Label, font, label, Palette.Text, Line);
        if (item.Detail is { Length: > 0 } detail && label.Right + this.Px(10) < right)
            TextRenderer.DrawText(g, detail, UiFonts.Get(11.5f, DeviceDpi),
                Rectangle.FromLTRB(label.Right + this.Px(10), row.Y, right, row.Bottom), Palette.TextFaint, Line | TextFormatFlags.PathEllipsis);
    }
}
