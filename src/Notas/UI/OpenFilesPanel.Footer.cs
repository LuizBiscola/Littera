using Notas.Theme;

namespace Notas.UI;

internal sealed partial class OpenFilesPanel
{
    private void PaintFooter(Graphics g, int dpi)
    {
        var footer = Rectangle.FromLTRB(0, Height - this.Px(FooterHeight), Width - 1, Height);
        using (var back = new SolidBrush(Palette.Panel))
            g.FillRectangle(back, footer); // lista rolada não invade o rodapé
        var position = new Point(this.Px(16), footer.Top + this.Px(10));
        Font keys = UiFonts.Semibold(10.5f, dpi);
        TextRenderer.DrawText(g, "Ctrl+Tab", keys, position, Palette.TextMuted, Plain);
        position.X += TextRenderer.MeasureText(g, "Ctrl+Tab", keys, Size.Empty, Plain).Width + this.Px(6);
        TextRenderer.DrawText(g, "alternar abas", UiFonts.Get(10.5f, dpi), position, Palette.TextFaint, Plain);
    }
}
