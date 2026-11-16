using Notas.Editor;
using Notas.Services;

namespace Notas.UI;

internal sealed partial class MainForm
{
    /// <summary>Acima disso o rascunho automático espera o fechamento: copiar o texto a cada pausa custaria caro.</summary>
    private const int AutosaveLimit = 5_000_000;

    private readonly System.Windows.Forms.Timer _draftTimer = new();

    /// <summary>Cada edição reinicia a contagem; o rascunho é gravado quando o usuário para de digitar.</summary>
    private void ScheduleDrafts()
    {
        _draftTimer.Interval = Math.Max(1, _settings.Files.AutosaveSeconds) * 1000;
        _draftTimer.Stop();
        _draftTimer.Start();
    }

    private void SaveDrafts(int maxChars)
    {
        _draftTimer.Stop();
        foreach (OpenDocument document in _documents.Documents)
        {
            if (document is not { IsLoaded: true, Editor: { } editor } || editor.TextLength > maxChars)
                continue;

            if (editor.Modified || document.FormatChanged)
            {
                document.DraftId ??= Guid.NewGuid().ToString("N");
                try
                {
                    DraftStore.Write(document.DraftId, editor.Text);
                }
                catch (IOException ex)
                {
                    ErrorReport.Log(ex); // disco cheio ou bloqueado: tenta de novo na próxima pausa
                }
            }
            else if (!document.HasUnsavedDraft && document.DraftId is not null)
            {
                DraftStore.Delete(document.DraftId); // desfez até o ponto salvo: rascunho sobrando
                document.DraftId = null;
            }
        }
    }
}
