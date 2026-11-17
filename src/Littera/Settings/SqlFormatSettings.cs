namespace Littera.Settings;

/// <summary>Opções do formatador de SQL (Ctrl+Shift+F); o recuo vem das configurações do editor.</summary>
internal sealed class SqlFormatSettings
{
    /// <summary>"upper", "lower" ou "keep" (como foi digitado).</summary>
    public string KeywordCase { get; set; } = "upper";

    /// <summary>Uma coluna do SELECT por linha, alinhadas abaixo da primeira.</summary>
    public bool OneColumnPerLine { get; set; } = true;

    /// <summary>AND/OR do WHERE em linhas próprias, alinhados à direita da cláusula.</summary>
    public bool BreakConditions { get; set; } = true;
}
