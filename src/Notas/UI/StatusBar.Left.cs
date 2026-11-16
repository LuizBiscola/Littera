using Notas.Theme;

namespace Notas.UI;

internal sealed partial class StatusBar
{
    private const TextFormatFlags Plain = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix
        | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

    // Medir sem reticências: com EndEllipsis e tamanho vazio o GDI devolve a largura já cortada.
    private const TextFormatFlags Measure = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;

    /// <summary>Posição do cursor, bloco atual (faixa azul) e ocorrências (amostra amarela), como no protótipo.</summary>
    private void PaintLeft(Graphics g, Font font, int limit)
    {
        int x = this.Px(14);
        x = PaintText(g, _position, font, Palette.Text, x, limit);

        if (_block is not null && x < limit)
        {
            using var accent = new SolidBrush(Palette.Accent);
            g.FillRectangle(accent, x, (Height - this.Px(12)) / 2, this.Px(3), this.Px(12));
            x = PaintText(g, _block, font, Palette.TextMuted, x + this.Px(9), limit);
        }

        x = PaintFolded(g, font, x, limit);

        if (_occurrences is not null && x < limit)
        {
            var swatch = new Rectangle(x, (Height - this.Px(10)) / 2, this.Px(10), this.Px(10));
            Shapes.FillRounded(g, Palette.MatchBorder, swatch, this.Px(2));
            swatch.Inflate(-1, -1);
            Shapes.FillRounded(g, Palette.Match, swatch, this.Px(2));
            PaintText(g, _occurrences, font, Palette.TextMuted, x + this.Px(16), limit);
        }
    }

    private int PaintText(Graphics g, string? text, Font font, Color color, int x, int limit)
    {
        if (string.IsNullOrEmpty(text) || x >= limit)
            return x;

        int width = Math.Min(TextRenderer.MeasureText(g, text, font, Size.Empty, Measure).Width, limit - x);
        TextRenderer.DrawText(g, text, font, new Rectangle(x, 0, width, Height), color, Plain);
        return x + width + this.Px(18);
    }
}
