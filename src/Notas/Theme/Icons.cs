namespace Notas.Theme;

/// <summary>Glifos da fonte Segoe Fluent Icons (mesmos códigos na Segoe MDL2 Assets): sem arquivos de imagem.</summary>
internal static class Icons
{
    public const char Logo = '';
    public const char PanelLeft = '';
    public const char PanelRight = '';
    public const char Search = '';
    public const char Settings = '';
    public const char Add = '';
    public const char Close = '';
    public const char Document = '';
    public const char ChevronDown = '';
    public const char ChevronUp = '';
    public const char ChevronRight = '';
    public const char ArrowUp = '';
    public const char ArrowDown = '';
    public const char Minimize = '';
    public const char Maximize = '';
    public const char Restore = '';
    public const char Refresh = '';
    public const char CollapseAll = '';
    public const char Folder = '';
    public const char FolderOpen = '';
    public const char ChevronDownSmall = '';
    public const char View = '';
    public const char Copy = '';
    public const char Undo = '\uE7A7';
    public const char Pin = '';
    public const char Unpin = '';
    public const char Dismiss = '';
    public const char DismissThin = '';
    public const char CheckMark = '';
    public const char Clipboard = '';
    public const char OpenOutside = '';
    public const char Edit = '';
    public const char ChevronRightSmall = '';

    public static void Draw(Graphics g, char glyph, Rectangle bounds, Color color, float px, int dpi)
    {
        TextRenderer.DrawText(g, glyph.ToString(), UiFonts.Icon(px, dpi), bounds, color,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
    }
}
