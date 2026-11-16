using Notas.Editor.Languages;
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

    public Language Language { get; set; } = LanguageRegistry.PlainText;

    /// <summary>Escolhida à mão no seletor: não muda mais pela extensão (nem ao "Salvar como").</summary>
    public bool LanguageChosen { get; set; }

    /// <summary>O conteúdo do arquivo já está no editor (documentos novos nascem carregados).</summary>
    public bool IsLoaded { get; set; }

    public bool IsLoading => LoadCancellation is not null;

    /// <summary>Cancelada ao fechar a aba no meio da carga.</summary>
    public CancellationTokenSource? LoadCancellation { get; set; }

    /// <summary>Encoding trocado pela barra de status: o texto é o mesmo, mas o arquivo precisa ser regravado.</summary>
    public bool FormatChanged { get; set; }

    public bool IsDirty => (Editor?.Modified ?? false) || FormatChanged || HasUnsavedDraft;

    /// <summary>Id do rascunho em rascunhos\ (alterações guardadas fora do arquivo).</summary>
    public string? DraftId { get; set; }

    /// <summary>Veio de um rascunho da sessão anterior: está alterado mesmo sem edição nova.</summary>
    public bool HasUnsavedDraft { get; set; }

    /// <summary>De onde ler ao carregar quando não é o próprio arquivo (o rascunho).</summary>
    public string? SourcePath { get; set; }

    /// <summary>Cursor e rolagem da sessão anterior, aplicados quando o arquivo terminar de carregar.</summary>
    public Settings.SessionDocument? Restore { get; set; }
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
