using Littera.Services;
using ScintillaNET;

namespace Littera.Editor;

internal sealed partial class DocumentManager
{
    /// <summary>Arquivo carregado: a janela aplica linguagem e posição salva.</summary>
    public event Action<OpenDocument>? Loaded;

    /// <summary>Falha de leitura (arquivo sumiu, sem permissão): a janela avisa e fecha a aba.</summary>
    public event Action<OpenDocument, Exception>? LoadFailed;

    private async Task LoadAsync(OpenDocument document)
    {
        Scintilla editor = document.Editor!;
        var cancellation = new CancellationTokenSource();
        document.LoadCancellation = cancellation;
        DetectLanguage(document);
        Changed?.Invoke();
        try
        {
            TextFileReader reader = await TextFileReader.OpenAsync(document.SourcePath ?? document.Path!, cancellation.Token);
            LineEnding lineEnding = await DocumentLoader.LoadAsync(editor, reader.Chunks, cancellation.Token);
            document.Format = new TextFormat(reader.Encoding, lineEnding);
            editor.EolMode = lineEnding.ToScintilla();
            document.IsLoaded = true;
            document.SourcePath = null;
            RestorePosition(document, editor);
            Loaded?.Invoke(document);
        }
        catch (OperationCanceledException)
        {
            // Aba fechada durante a carga.
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            LoadFailed?.Invoke(document, ex);
        }
        finally
        {
            document.LoadCancellation = null;
            cancellation.Dispose();
            Changed?.Invoke();
        }
    }

    /// <summary>Volta cursor, seleção e rolagem salvos na sessão (limitados ao tamanho atual do arquivo).</summary>
    private static void RestorePosition(OpenDocument document, Scintilla editor)
    {
        if (document.Restore is not { } saved)
            return;

        int length = editor.TextLength;
        editor.SetSelection(Math.Clamp(saved.CaretPosition, 0, length), Math.Clamp(saved.AnchorPosition, 0, length));
        editor.FirstVisibleLine = Math.Max(0, saved.FirstVisibleLine);
        if (saved.Folded is { Count: > 0 } folded)
            document.PendingFolds = new Folding.FoldRestore(folded, Math.Max(0, saved.FirstVisibleLine));
        if (document.HasUnsavedDraft && Enum.TryParse(saved.Encoding, out TextEncodingKind encoding) && Enum.TryParse(saved.LineEnding, out LineEnding ending))
            document.Format = new TextFormat(encoding, ending); // rascunho é UTF-8; vale o formato do arquivo original
        document.Restore = null;
    }
}
