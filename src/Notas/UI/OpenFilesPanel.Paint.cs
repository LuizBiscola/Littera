using Notas.Theme;

namespace Notas.UI;

internal sealed partial class OpenFilesPanel
{
    private const TextFormatFlags Plain = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        int dpi = DeviceDpi;
        ClampScroll(); // abas fechadas ou janela menor
        g.Clear(Palette.Panel);
        using (var border = new Pen(Palette.Border))
        {
            g.DrawLine(border, Width - 1, 0, Width - 1, Height);
            if (_documents.PinnedCount > 0)
                g.DrawLine(border, this.Px(16), PinnedArea.Bottom + this.Px(10), Width - this.Px(16), PinnedArea.Bottom + this.Px(10));
        }

        if (_documents.PinnedCount > 0)
            PaintSectionHeader(g, "FIXADAS", _documents.PinnedCount, this.Px(15), withPin: true, dpi);
        PaintSectionHeader(g, "ABERTOS", _documents.Documents.Count - _documents.PinnedCount, OpenHeaderTop + this.Px(18), withPin: false, dpi);
        if (_hoverNew)
            Shapes.FillRounded(g, Palette.ButtonHover, NewButtonBounds, this.Px(4));
        Icons.Draw(g, Icons.Add, NewButtonBounds, Palette.TextMuted, 11, dpi);

        foreach (TabSlot slot in Slots())
        {
            if (slot.Document == _drag)
                continue; // a aba arrastada é desenhada solta, embaixo do mouse
            g.SetClip(slot.Pinned ? PinnedArea : OpenArea);
            PaintItem(g, slot, dpi);
            g.ResetClip();
        }

        PaintDrag(g, dpi);
        PaintFooter(g, dpi);
    }
}
