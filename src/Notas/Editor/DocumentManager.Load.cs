using Notas.Services;
using ScintillaNET;

namespace Notas.Editor;

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
        Changed?.Invoke();
        try
        {
            TextFileReader reader = await TextFileReader.OpenAsync(document.Path!, cancellation.Token);
            LineEnding lineEnding = await DocumentLoader.LoadAsync(editor, reader.Chunks, cancellation.Token);
            document.Format = new TextFormat(reader.Encoding, lineEnding);
            editor.EolMode = lineEnding.ToScintilla();
            document.IsLoaded = true;
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
}
