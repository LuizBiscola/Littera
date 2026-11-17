namespace Notas.UI.Chrome;

/// <summary>Botão desenhado pela própria barra de título (sem controle filho, sem janela extra).</summary>
internal sealed class ChromeButton(char glyph, string toolTip, Action onClick)
{
    public char Glyph { get; set; } = glyph;

    public string ToolTip { get; } = toolTip;

    public Action OnClick { get; } = onClick;

    public Rectangle Bounds { get; set; }

    /// <summary>Ligado (ex.: painel visível): fundo azul claro e ícone azul, como no protótipo.</summary>
    public bool Toggled { get; set; }

    /// <summary>Minimizar/maximizar/fechar: 46×40, sem cantos arredondados.</summary>
    public bool IsWindowButton { get; init; }

    public bool IsClose { get; init; }

    /// <summary>Nome ao lado do ícone (a marca "Littera", que abre o menu do app).</summary>
    public string? Label { get; init; }
}
