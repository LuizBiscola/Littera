using Notas.Theme;

namespace Notas.UI;

internal sealed partial class ChangePreview
{
    /// <summary>Removidas (fundo vermelho claro, "-") e depois as novas (verde claro, "+"), com os números de cada versão.</summary>
    private void PaintRows(Graphics g, Rectangle area)
    {
        using var font = new Font(UiFonts.FirstInstalled("Cascadia Mono", "Consolas"), 11.5f * DeviceDpi / 96f, GraphicsUnit.Pixel);
        int y = area.Top + this.Px(6), shown = 0;
        for (int i = 0; i < _removed.Length && shown < MaxRows; i++, shown++, y += this.Px(RowHeight))
            PaintRow(g, font, area, y, _hunk.BeforeStart + i + 1, '-', _removed[i], removed: true);
        for (int i = 0; i < _added.Length && shown < MaxRows; i++, shown++, y += this.Px(RowHeight))
            PaintRow(g, font, area, y, _hunk.AfterStart + i + 1, '+', _added[i], removed: false);

        int hidden = _removed.Length + _added.Length - shown;
        if (hidden > 0)
            TextRenderer.DrawText(g, $"… mais {hidden} linhas", font, new Rectangle(area.Left + this.Px(52), y, area.Width, this.Px(RowHeight)), Palette.TextFaint, Line);
    }

    private void PaintRow(Graphics g, Font font, Rectangle area, int y, int number, char sign, string text, bool removed)
    {
        var row = new Rectangle(area.Left, y, area.Width, this.Px(RowHeight));
        using (var back = new SolidBrush(removed ? Palette.DiffRemovedBack : Palette.DiffAddedBack))
            g.FillRectangle(back, row);

        TextRenderer.DrawText(g, number.ToString(), font, new Rectangle(row.X, y, this.Px(27), row.Height),
            removed ? Palette.DiffRemovedNumber : Palette.DiffAddedNumber, Line | TextFormatFlags.Right);
        TextRenderer.DrawText(g, sign.ToString(), font, new Rectangle(row.X + this.Px(35), y, this.Px(12), row.Height),
            removed ? Palette.DiffRemovedSign : Palette.DiffAddedSign, Line);
        TextRenderer.DrawText(g, text.Replace("\t", "    "), font, Rectangle.FromLTRB(row.X + this.Px(52), y, row.Right - this.Px(8), row.Bottom),
            removed ? Palette.DiffRemovedText : Palette.DiffAddedText, Line);
    }
}
