namespace Notas.Theme;

/// <summary>Cores da interface, tiradas das variáveis do protótipo no pen.dev.</summary>
internal static class Palette
{
    public static readonly Color Background = Hex(0xFFFFFF);
    public static readonly Color Panel = Hex(0xF7F7F8);
    public static readonly Color Border = Hex(0xE6E7EA);

    public static readonly Color Text = Hex(0x1F2328);
    public static readonly Color TextSecondary = Hex(0x4A4F57);
    public static readonly Color TextMuted = Hex(0x6B7079);
    public static readonly Color TextFaint = Hex(0xA2A6AD);

    public static readonly Color Accent = Hex(0x2F6FEB);
    public static readonly Color AccentSoft = Hex(0xEAF1FE);

    public static readonly Color ItemHover = Hex(0xF1F2F4);
    public static readonly Color ItemActive = Hex(0xEEEFF1);
    public static readonly Color IconActive = Hex(0x5B6270);
    public static readonly Color IconInactive = Hex(0x9AA0A8);
    public static readonly Color CloseInactive = Hex(0xB4B8BF);
    public static readonly Color Dirty = Hex(0xE0892B);
    public static readonly Color WindowCloseHover = Hex(0xC42B1C);

    // Editor
    public static readonly Color Selection = Hex(0xD3E1FC);
    public static readonly Color StatementBackground = Hex(0xF7F9FE);
    public static readonly Color StatementMarker = Hex(0xB9CDF7);
    public static readonly Color LineNumber = Hex(0xB4B8BF);
    public static readonly Color LineNumberInBlock = Hex(0x8794AD);
    public static readonly Color Match = Hex(0xFCE7A6);
    public static readonly Color MatchBorder = Hex(0xE9C65A);
    public static readonly Color MatchCurrent = Hex(0xF8D774);

    public static Color Hex(int rgb) => Color.FromArgb((rgb >> 16) & 0xFF, (rgb >> 8) & 0xFF, rgb & 0xFF);
}
