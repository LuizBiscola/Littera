using Notas.Editor;
using Notas.Services;

namespace Notas.UI;

internal sealed partial class MainForm
{
    /// <summary>Salva (pede o nome se for novo ou em "Salvar como"). Devolve false se cancelou ou falhou.</summary>
    private async Task<bool> SaveAsync(OpenDocument document, bool askPath = false)
    {
        if (document.IsLoading)
            return false;

        string? path = document.Path;
        if (askPath || path is null)
        {
            using SaveFileDialog dialog = SaveDialogFor(document);
            if (dialog.ShowDialog(this) != DialogResult.OK)
                return false;
            path = dialog.FileName;
        }

        try
        {
            await _documents.SaveAsync(document, path);
            RecentFiles.Add(_settings.RecentFiles, document.Path!);
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show(this, $"Não foi possível salvar:\n{ex.Message}", "Notas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
    }
}
