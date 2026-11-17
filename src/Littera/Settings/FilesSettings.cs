namespace Littera.Settings;

/// <summary>Arquivos e rascunhos.</summary>
internal sealed class FilesSettings
{
    /// <summary>Ao fechar, guarda as abas alteradas como rascunho em vez de perguntar (nada se perde).</summary>
    public bool KeepDraftsOnClose { get; set; } = true;

    /// <summary>Segundos sem digitar até gravar o rascunho das abas alteradas.</summary>
    public int AutosaveSeconds { get; set; } = 3;

    /// <summary>Formato de arquivos novos: "Utf8", "Utf8Bom" ou "Ansi"; "CrLf" ou "Lf".</summary>
    public string NewFileEncoding { get; set; } = "Utf8";

    public string NewFileLineEnding { get; set; } = "CrLf";
}
