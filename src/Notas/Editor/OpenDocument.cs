using Notas.Services;
using ScintillaNET;

namespace Notas.Editor;

/// <summary>Um arquivo aberto (ou "Sem título"). O editor só é criado quando a aba é vista pela primeira vez.</summary>
internal sealed class OpenDocument(string? path, int untitledNumber = 0)
{
    public string? Path { get; set; } = path;

    public int UntitledNumber { get; } = untitledNumber;

    public string Title => Path is null ? $"Sem título {UntitledNumber}" : System.IO.Path.GetFileName(Path);

    public Scintilla? Editor { get; set; }

    public TextFormat Format { get; set; } = TextFormat.Default;

    /// <summary>O conteúdo do arquivo já está no editor (documentos novos nascem carregados).</summary>
    public bool IsLoaded { get; set; }

    public bool IsLoading => LoadCancellation is not null;

    /// <summary>Cancelada ao fechar a aba no meio da carga.</summary>
    public CancellationTokenSource? LoadCancellation { get; set; }

    /// <summary>Encoding trocado pela barra de status: o texto é o mesmo, mas o arquivo precisa ser regravado.</summary>
    public bool FormatChanged { get; set; }

    public bool IsDirty => (Editor?.Modified ?? false) || FormatChanged;
}

internal static class LineEndingExtensions
{
    public static Eol ToScintilla(this LineEnding ending) => ending switch
    {
        LineEnding.Lf => Eol.Lf,
        LineEnding.Cr => Eol.Cr,
        _ => Eol.CrLf,
    };
}
