using Littera.Theme;

namespace Littera.UI;

internal sealed partial class OpenFilesPanel
{
    /// <summary>× (sempre visível nas comuns) e alfinete; devolve onde começam, para a bolinha e o nome pararem antes.</summary>
    private int PaintButtons(Graphics g, TabSlot slot, bool hover, int dpi)
    {
        if (slot.Pinned)
        {
            bool overPin = hover && _hoverButton == TabButton.Pin;
            PaintButton(g, PinBounds(slot), overPin ? Icons.Unpin : Icons.Pin, overPin, Palette.IconInactive, dpi);
            return PinBounds(slot).Left;
        }

        Rectangle close = CloseBounds(slot.Bounds);
        bool overClose = hover && _hoverButton == TabButton.Close;
        PaintButton(g, close, Icons.Close, overClose, hover || slot.Document == _documents.Active ? Palette.TextMuted : Palette.CloseInactive, dpi, 9);
        if (!hover)
            return close.Left;

        PaintButton(g, PinBounds(slot), Icons.Pin, _hoverButton == TabButton.Pin, Palette.IconInactive, dpi);
        return PinBounds(slot).Left;
    }

    /// <summary>Botão 20×20: com o mouse em cima ganha fundo e o ícone escurece.</summary>
    private void PaintButton(Graphics g, Rectangle bounds, char glyph, bool hot, Color rest, int dpi, float size = 10)
    {
        if (hot)
            Shapes.FillRounded(g, Palette.ButtonHover, bounds, this.Px(4));
        Icons.Draw(g, glyph, bounds, hot ? Palette.Text : rest, size, dpi);
    }
}
