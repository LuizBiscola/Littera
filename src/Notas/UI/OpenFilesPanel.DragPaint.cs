using Notas.Theme;

namespace Notas.UI;

internal sealed partial class OpenFilesPanel
{
    /// <summary>Linha azul de 2 px onde a aba vai cair e a aba solta embaixo do mouse, com sombra e leve inclinação.</summary>
    private void PaintDrag(Graphics g, int dpi)
    {
        if (_drag is null)
            return;

        if (_drop is { } drop)
        {
            List<Rectangle> rows = [.. Slots().Where(s => s.Pinned == drop.Pinned && s.Document != _drag).Select(s => s.Bounds)];
            Rectangle area = drop.Pinned ? PinnedArea : OpenArea;
            int y = drop.Index < rows.Count ? rows[drop.Index].Top - this.Px(1)
                : rows.Count > 0 ? rows[^1].Bottom + this.Px(1) : area.Top + this.Px(2);
            using var line = new SolidBrush(Palette.Accent);
            g.FillRectangle(line, this.Px(8), y - this.Px(1), Width - this.Px(16), this.Px(2));
        }

        var ghost = new Rectangle(this.Px(8), _dragPoint.Y - this.Px(16), Width - this.Px(16), this.Px(ItemHeight));
        var state = g.Save();
        g.TranslateTransform(ghost.X + ghost.Width / 2f, ghost.Y + ghost.Height / 2f);
        g.RotateTransform(-1.5f);
        g.TranslateTransform(-(ghost.X + ghost.Width / 2f), -(ghost.Y + ghost.Height / 2f));
        Shapes.FillRounded(g, Color.FromArgb(40, 0, 0, 0), new Rectangle(ghost.X + this.Px(2), ghost.Y + this.Px(3), ghost.Width, ghost.Height), this.Px(6));
        Shapes.FillRounded(g, Palette.Border, ghost, this.Px(6));
        Rectangle inner = Rectangle.Inflate(ghost, -1, -1);
        Shapes.FillRounded(g, Palette.Background, inner, this.Px(5));
        TabIcon(_drag).Draw(g, new Rectangle(ghost.X + this.Px(10), ghost.Y + this.Px(9), this.Px(15), this.Px(15)), Palette.IconActive);
        TextRenderer.DrawText(g, _drag.Title, UiFonts.Get(13, dpi),
            Rectangle.FromLTRB(ghost.X + this.Px(33), ghost.Y, ghost.Right - this.Px(10), ghost.Bottom), Palette.Text, LeftText);
        g.Restore(state);
    }
}
