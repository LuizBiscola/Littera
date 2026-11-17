namespace Littera.UI.Find;

/// <summary>Botão desenhado no painel de busca: ícone (glifo) ou texto curto ("Aa", "Arquivo").</summary>
internal sealed class FindButton(string? text, char glyph, string toolTip, Action onClick)
{
    public string? Text { get; } = text;

    public char Glyph { get; set; } = glyph;

    public string ToolTip { get; } = toolTip;

    public Action OnClick { get; } = onClick;

    public Rectangle Bounds { get; set; }

    public bool Toggled { get; set; }

    public bool Visible { get; set; } = true;

    public static FindButton Icon(char glyph, string toolTip, Action onClick) => new(null, glyph, toolTip, onClick);

    public static FindButton Label(string text, string toolTip, Action onClick) => new(text, '\0', toolTip, onClick);
}
