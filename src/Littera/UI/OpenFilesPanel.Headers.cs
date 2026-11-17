using Littera.Theme;

namespace Littera.UI;

internal sealed partial class OpenFilesPanel
{
    /// <summary>"FIXADAS 2" (com alfinete) ou "ABERTOS 4", com o contador num selo cinza.</summary>
    private void PaintSectionHeader(Graphics g, string text, int count, int y, bool withPin, int dpi)
    {
        int x = this.Px(16);
        if (withPin)
        {
            Icons.Draw(g, Icons.Pin, new Rectangle(x, y + this.Px(1), this.Px(11), this.Px(11)), Palette.TextMuted, 9, dpi);
            x = this.Px(33);
        }

        Font label = UiFonts.Semibold(10.5f, dpi);
        TextRenderer.DrawText(g, text, label, new Point(x, y), Palette.TextMuted, Plain);
        string number = count.ToString();
        Font badgeFont = UiFonts.Semibold(10f, dpi);
        x += TextRenderer.MeasureText(g, text, label, Size.Empty, Plain).Width + this.Px(6);
        int width = TextRenderer.MeasureText(g, number, badgeFont, Size.Empty, Plain).Width + this.Px(12);
        var badge = new Rectangle(x, y - this.Px(1), width, this.Px(14));
        Shapes.FillRounded(g, Palette.Border, badge, this.Px(7));
        TextRenderer.DrawText(g, number, badgeFont, badge, Palette.TextMuted, Plain | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }
}
