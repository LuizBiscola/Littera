using System.Drawing.Drawing2D;
using Littera.Services;
using Littera.Theme;

namespace Littera.UI;

internal sealed partial class FolderPanel
{
    /// <summary>No resultado do filtro, a subpasta aparece em cinza depois do nome.</summary>
    private void PaintLocation(Graphics g, FolderEntry entry, Font font, Rectangle text, int dpi)
    {
        string? folder = Path.GetDirectoryName(Path.GetRelativePath(Folder!, entry.Path));
        // Medir sem EndEllipsis: com ele o GDI devolve uma largura menor que a do texto.
        int nameWidth = TextRenderer.MeasureText(g, entry.Name, font, Size.Empty, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Width;
        if (string.IsNullOrEmpty(folder) || nameWidth >= text.Width)
            return;
        TextRenderer.DrawText(g, folder, UiFonts.Get(11, dpi),
            Rectangle.FromLTRB(text.X + nameWidth + this.Px(6), text.Y, text.Right, text.Bottom), Palette.TextFaint, Line);
    }

    private void PaintDot(Graphics g, Rectangle row, Color color, int size)
    {
        using var brush = new SolidBrush(color);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        int px = this.Px(size);
        g.FillEllipse(brush, row.Right - this.Px(13) - px / 2, row.Y + (row.Height - px) / 2, px, px);
        g.SmoothingMode = SmoothingMode.Default;
    }

    private static bool SamePath(string? a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
}
