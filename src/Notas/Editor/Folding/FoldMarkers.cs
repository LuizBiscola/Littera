using System.Drawing.Imaging;
using System.Drawing.Text;
using Notas.Theme;
using ScintillaNET;

namespace Notas.Editor.Folding;

/// <summary>Setas da coluna de dobra do protótipo: glifos da Segoe Fluent desenhados em imagem (uma por DPI).</summary>
internal static class FoldMarkers
{
    private static readonly Dictionary<int, (Bitmap Open, Bitmap Closed)> Cache = [];

    public static void Define(Scintilla editor)
    {
        int dpi = editor.DeviceDpi;
        if (!Cache.TryGetValue(dpi, out var images))
        {
            int size = (int)Math.Round(14 * dpi / 96.0);
            images = (Draw(Icons.ChevronDownSmall, Palette.LineNumber, null, size, dpi),
                Draw(Icons.ChevronRightSmall, Palette.TextMuted, Palette.FoldedMarker, size, dpi));
            Cache[dpi] = images;
        }

        foreach (int marker in new[] { Marker.FolderOpen, Marker.FolderOpenMid })
            editor.Markers[marker].DefineRgbaImage(images.Open);
        foreach (int marker in new[] { Marker.Folder, Marker.FolderEnd })
            editor.Markers[marker].DefineRgbaImage(images.Closed);
        foreach (int marker in new[] { Marker.FolderSub, Marker.FolderTail, Marker.FolderMidTail })
            editor.Markers[marker].Symbol = MarkerSymbol.Empty;
    }

    private static Bitmap Draw(char glyph, Color color, Color? background, int size, int dpi)
    {
        var image = new Bitmap(size, size, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(image);
        if (background is { } back)
            Shapes.FillRounded(g, back, new Rectangle(0, 0, size, size), size / 4);
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit; // ClearType não funciona em imagem transparente
        using var brush = new SolidBrush(color);
        using var center = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString(glyph.ToString(), UiFonts.Icon(9, dpi), brush, new RectangleF(0, 0, size, size), center);
        return image;
    }
}
