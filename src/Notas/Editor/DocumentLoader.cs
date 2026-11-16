using System.Diagnostics;
using System.Threading.Channels;
using Notas.Services;
using ScintillaNET;

namespace Notas.Editor;

/// <summary>
/// Anexa o texto ao editor em blocos, devolvendo a vez à fila de mensagens a cada ~25 ms.
/// Não usamos ILoader + troca de documento: o Scintilla.NET reconstrói seu índice de caracteres
/// inteiro na thread de UI nessa troca (~0,5 s para 50 MB). Aqui o índice cresce bloco a bloco.
/// </summary>
internal static class DocumentLoader
{
    private const int SCI_SETUNDOCOLLECTION = 2012;
    private const int SliceMs = 25;

    /// <summary>Devolve o fim de linha detectado no primeiro bloco.</summary>
    public static async Task<LineEnding> LoadAsync(Scintilla editor, ChannelReader<string> chunks, CancellationToken ct)
    {
        editor.DirectMessage(SCI_SETUNDOCOLLECTION, IntPtr.Zero);
        LineEnding? lineEnding = null;
        var slice = Stopwatch.StartNew();
        try
        {
            await foreach (string chunk in chunks.ReadAllAsync(ct))
            {
                lineEnding ??= EolDetector.Detect(chunk);
                editor.ReadOnly = false; // só leitura entre blocos: o usuário não edita no meio da carga
                editor.AppendText(chunk);
                editor.ReadOnly = true;

                if (slice.ElapsedMilliseconds >= SliceMs)
                {
                    await UiIdle.NextAsync();
                    slice.Restart();
                }
            }
        }
        finally
        {
            editor.ReadOnly = false;
            editor.DirectMessage(SCI_SETUNDOCOLLECTION, new IntPtr(1));
            editor.EmptyUndoBuffer();
            editor.SetSavePoint();
        }

        return lineEnding ?? LineEnding.CrLf;
    }
}
