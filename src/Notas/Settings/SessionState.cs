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

    /// <summary>Rascunho com alterações não salvas (arquivo em rascunhos\); vazio = abre do disco.</summary>
    public string? Draft { get; set; }

    /// <summary>"Sem título N" (só quando Path está vazio).</summary>
    public int UntitledNumber { get; set; }

    /// <summary>Formato do arquivo original: o rascunho é sempre UTF-8, mas salvar deve manter o formato.</summary>
    public string? Encoding { get; set; }

    public string? LineEnding { get; set; }
}
