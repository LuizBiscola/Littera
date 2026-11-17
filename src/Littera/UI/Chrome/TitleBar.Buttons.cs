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

        if (button.Label is not null)
        {
            // Marca do protótipo: logo 18×18 em (12, 11) e o nome a partir de x 39, com a linha de base em y 26,2.
            var glyph = new Rectangle(button.Bounds.X + this.Px(6), button.Bounds.Y + this.Px(5), this.Px(18), this.Px(18));
            LogoMark.Draw(g, glyph);
            Wordmark.Draw(g, button.Label, new PointF(button.Bounds.X + this.Px(33f), button.Bounds.Y + this.Px(20.2f)), dpi);
            return;
        }

        Icons.Draw(g, button.Glyph, button.Bounds, button.Toggled ? Palette.Accent : Palette.TextMuted, 14, dpi);
    }

    /// <summary>Troca o ícone entre maximizar e restaurar conforme o estado da janela.</summary>
    public void SetMaximized(bool maximized)
    {
        _maximize.Glyph = maximized ? Icons.Restore : Icons.Maximize;
        Invalidate(_maximize.Bounds);
    }
}
