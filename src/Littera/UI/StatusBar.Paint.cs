using Littera.Theme;

namespace Littera.UI;

internal sealed partial class StatusBar
{
    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.Clear(Palette.Panel);
        using (var border = new Pen(Palette.Border))
            g.DrawLine(border, 0, 0, Width, 0);

        Font font = UiFonts.Get(11.5f, DeviceDpi);
        int limit = PaintSegments(g, font);
        PaintLeft(g, font, limit);
    }

    /// <summary>Da direita para a esquerda; devolve onde terminam, para o lado esquerdo não invadir.</summary>
    private int PaintSegments(Graphics g, Font font)
    {
        int x = Width - this.Px(14);
        foreach (StatusSegment segment in RightSegments)
        {
            if (!_segments.TryGetValue(segment, out string? text) || text.Length == 0)
                continue;

            bool language = segment == StatusSegment.Language;
            Font segmentFont = language ? UiFonts.Semibold(11.5f, DeviceDpi) : font;
            int textWidth = TextRenderer.MeasureText(g, text, segmentFont, Size.Empty, Measure).Width;
            int width = textWidth + (language ? this.Px(14) : 0);
            x -= width;

            var bounds = new Rectangle(x - this.Px(6), this.Px(4), width + this.Px(12), Height - this.Px(7));
            _segmentBounds[segment] = bounds;
            if (_hover == segment)
                Shapes.FillRounded(g, Palette.ItemActive, bounds, this.Px(4));

            TextRenderer.DrawText(g, text, segmentFont, new Rectangle(x, 0, textWidth, Height), language ? Palette.Text : Palette.TextMuted, Plain);
            if (language)
                Icons.Draw(g, Icons.ChevronDown, new Rectangle(x + textWidth + this.Px(3), 0, this.Px(10), Height), Palette.TextMuted, 8, DeviceDpi);
            x -= this.Px(18);
        }

        return x;
    }
}
