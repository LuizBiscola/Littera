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
    public static readonly Color ButtonHover = Hex(0xE3E5E9);
    public static readonly Color PinFlash = Hex(0xE4ECFD);
    public static readonly Color Tooltip = Hex(0x1F2328);
    public static readonly Color IconActive = Hex(0x5B6270);
    public static readonly Color IconInactive = Hex(0x9AA0A8);
    public static readonly Color CloseInactive = Hex(0xB4B8BF);
    public static readonly Color Dirty = Hex(0xE0892B);
    public static readonly Color FolderIcon = Hex(0x8C939D);
    public static readonly Color OpenInTab = Hex(0xC3C7CD);
    public static readonly Color WindowCloseHover = Hex(0xC42B1C);

    // Editor
    public static readonly Color Selection = Hex(0xD3E1FC);
    public static readonly Color StatementBackground = Hex(0xF7F9FE);
    public static readonly Color StatementMarker = Hex(0xB9CDF7);
    public static readonly Color LineNumber = Hex(0xB4B8BF);
    public static readonly Color LineNumberInBlock = Hex(0x8794AD);
    public static readonly Color IndentGuide = Hex(0xDCDFE4);
    public static readonly Color FoldedMarker = Hex(0xECEDF0);
    public static readonly Color FoldSummary = Hex(0xF0F1F3);
    public static readonly Color FoldSummaryActive = Hex(0xE4ECFD);
    public static readonly Color CodePreview = Hex(0xFBFBFC);
    public static readonly Color ChangeUnsaved = Hex(0xE8892B);
    public static readonly Color ChangeSaved = Hex(0x3BAA6E);
    public static readonly Color RevertBack = Hex(0xFDF0E3);
    public static readonly Color RevertText = Hex(0xB8661A);
    public static readonly Color DiffRemovedBack = Hex(0xFDEEEE);
    public static readonly Color DiffRemovedNumber = Hex(0xC98A8A);
    public static readonly Color DiffRemovedSign = Hex(0xC0392B);
    public static readonly Color DiffRemovedText = Hex(0x8A3A3A);
    public static readonly Color DiffAddedBack = Hex(0xEAF7EF);
    public static readonly Color DiffAddedNumber = Hex(0x7DB394);
    public static readonly Color DiffAddedSign = Hex(0x23874F);
    public static readonly Color DiffAddedText = Hex(0x1F5E3A);
    public static readonly Color Match = Hex(0xFCE7A6);
    public static readonly Color MatchBorder = Hex(0xE9C65A);
    public static readonly Color MatchCurrent = Hex(0xF8D774);

    public static Color Hex(int rgb) => Color.FromArgb((rgb >> 16) & 0xFF, (rgb >> 8) & 0xFF, rgb & 0xFF);
}
