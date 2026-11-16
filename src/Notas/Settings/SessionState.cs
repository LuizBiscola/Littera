namespace Notas.Settings;

/// <summary>Conteúdo de %APPDATA%\Notas\session.json: o que estava aberto ao fechar.</summary>
internal sealed class SessionState
{
    public List<SessionDocument> Documents { get; set; } = [];

    public int ActiveIndex { get; set; }
}

internal sealed class SessionDocument
{
    public string Path { get; set; } = "";

    public int CaretPosition { get; set; }

    public int AnchorPosition { get; set; }

    public int FirstVisibleLine { get; set; }

    /// <summary>Linguagem escolhida manualmente; null = pela extensão do arquivo.</summary>
    public string? Language { get; set; }
}
