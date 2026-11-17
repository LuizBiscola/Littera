namespace Littera.Theme;

/// <summary>
/// Fontes da interface em pixels lógicos (como no protótipo), escaladas pelo DPI e guardadas em cache.
/// Inter não vem no Windows; usamos Segoe UI Variable (Windows 11) ou Segoe UI.
/// </summary>
internal static class UiFonts
{
    private static readonly Dictionary<(string, float, FontStyle, int), Font> Cache = [];

    public static string Family { get; } = FirstInstalled("Segoe UI Variable Text", "Segoe UI");

    public static string IconFamily { get; } = FirstInstalled("Segoe Fluent Icons", "Segoe MDL2 Assets");

    /// <summary>O protótipo usa peso 600; Bold do GDI+ seria 700, então procuramos a face Semibold.</summary>
    public static string SemiboldFamily { get; } =
        FirstInstalled("Segoe UI Variable Text Semibold", "Segoe UI Semibold", "Segoe UI");

    public static Font Get(float px, int dpi, FontStyle style = FontStyle.Regular) => Get(Family, px, dpi, style);

    public static Font Semibold(float px, int dpi) => Get(SemiboldFamily, px, dpi, FontStyle.Regular);

    public static Font Icon(float px, int dpi) => Get(IconFamily, px, dpi, FontStyle.Regular);

    private static Font Get(string family, float px, int dpi, FontStyle style)
    {
        var key = (family, px, style, dpi);
        if (!Cache.TryGetValue(key, out Font? font))
        {
            font = new Font(family, px * dpi / 96f, style, GraphicsUnit.Pixel);
            Cache[key] = font;
        }

        return font;
    }

    public static string FirstInstalled(params string[] families)
    {
        foreach (string name in families)
        {
            using var probe = new Font(name, 10f);
            if (string.Equals(probe.Name, name, StringComparison.OrdinalIgnoreCase))
                return name;
        }

        return families[^1];
    }
}
