using Notas.Theme;

namespace Notas.UI.Chrome;

internal sealed partial class TitleBar
{
    private void PaintButton(Graphics g, ChromeButton button, int dpi)
    {
        bool hot = button == _hover || (button == _maximize && _maximizeHot);

        if (button.IsWindowButton)
        {
            if (hot)
            {
                using var brush = new SolidBrush(button.IsClose ? Palette.WindowCloseHover : Palette.ItemHover);
                g.FillRectangle(brush, button.Bounds);
            }

            Color glyph = hot && button.IsClose ? Color.White : Palette.TextMuted;
            Icons.Draw(g, button.Glyph, button.Bounds, glyph, 10, dpi);
            return;
        }

        if (button.Toggled || hot)
            Shapes.FillRounded(g, button.Toggled ? Palette.AccentSoft : Palette.ItemHover, button.Bounds, this.Px(5));

        Icons.Draw(g, button.Glyph, button.Bounds, button.Toggled ? Palette.Accent : Palette.TextMuted, 14, dpi);
    }

    /// <summary>Troca o ícone entre maximizar e restaurar conforme o estado da janela.</summary>
    public void SetMaximized(bool maximized)
    {
        _maximize.Glyph = maximized ? Icons.Restore : Icons.Maximize;
        Invalidate(_maximize.Bounds);
    }
}
