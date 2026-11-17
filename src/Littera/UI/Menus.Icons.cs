namespace Notas.UI;

internal static partial class Menus
{
    private static readonly Dictionary<int, Bitmap> Placeholders = [];

    /// <summary>Ícone do item: o glifo vai em Tag e o renderizador o desenha na cor do estado (imagem só reserva o espaço).</summary>
    public static void SetIcon(ToolStripItem item, char glyph, int size)
    {
        if (!Placeholders.TryGetValue(size, out Bitmap? blank))
            Placeholders[size] = blank = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        item.Image = blank;
        item.Tag = glyph;
    }
}
