using Notas.Theme;

namespace Notas.UI;

internal sealed partial class OpenFilesPanel
{
    private const TextFormatFlags Plain = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        int dpi = DeviceDpi;
        g.Clear(Palette.Panel);
        using (var border = new Pen(Palette.Border))
            g.DrawLine(border, Width - 1, 0, Width - 1, Height);

        PaintHeader(g, dpi);
        g.SetClip(ListArea);
        for (int i = 0; i < _documents.Documents.Count; i++)
            PaintItem(g, i, dpi);
        g.ResetClip();
        PaintFooter(g, dpi);
    }

    private void PaintHeader(Graphics g, int dpi)
    {
        Font label = UiFonts.Semibold(10.5f, dpi);
        var position = new Point(this.Px(16), this.Px(18));
        TextRenderer.DrawText(g, "ABERTOS", label, position, Palette.TextMuted, Plain);

        string count = _documents.Documents.Count.ToString();
        Font badgeFont = UiFonts.Semibold(10f, dpi);
        int x = position.X + TextRenderer.MeasureText(g, "ABERTOS", label, Size.Empty, Plain).Width + this.Px(8);
        int width = TextRenderer.MeasureText(g, count, badgeFont, Size.Empty, Plain).Width + this.Px(12);
        var badge = new Rectangle(x, this.Px(17), width, this.Px(15));
        Shapes.FillRounded(g, Palette.Border, badge, this.Px(8));
        TextRenderer.DrawText(g, count, badgeFont, badge, Palette.TextMuted, Plain | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

        if (_hoverNew)
            Shapes.FillRounded(g, Palette.ItemActive, NewButtonBounds, this.Px(5));
        Icons.Draw(g, Icons.Add, NewButtonBounds, Palette.TextMuted, 12, dpi);
    }

    private void PaintFooter(Graphics g, int dpi)
    {
        var position = new Point(this.Px(16), Height - this.Px(FooterHeight) + this.Px(10));
        Font keys = UiFonts.Semibold(10.5f, dpi);
        TextRenderer.DrawText(g, "Ctrl+Tab", keys, position, Palette.TextMuted, Plain);
        position.X += TextRenderer.MeasureText(g, "Ctrl+Tab", keys, Size.Empty, Plain).Width + this.Px(6);
        TextRenderer.DrawText(g, "alternar abas", UiFonts.Get(10.5f, dpi), position, Palette.TextFaint, Plain);
    }
}
