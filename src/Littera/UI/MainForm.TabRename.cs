using Notas.Editor;
using Notas.Services;

namespace Notas.UI;

internal sealed partial class MainForm
{
    /// <summary>F2: o nome vira caixa de texto na própria aba (o painel aparece, se estava oculto).</summary>
    private void RenameTab(OpenDocument document)
    {
        if (_focusMode)
            ToggleFocusMode();
        if (!_settings.Window.ShowOpenFiles)
            ToggleOpenFiles();
        _openFiles.BeginRename(document);
    }

    /// <summary>Renomeia no disco; nome inválido ou já existente só avisa.</summary>
    private void RenameFile(OpenDocument document, string name)
    {
        if (document.Path is not { } path)
            return;
        if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            MessageBox.Show(this, "O nome não pode ter \\ / : * ? \" < > |", Product.Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string target = Path.Combine(Path.GetDirectoryName(path)!, name);
        bool sameFile = string.Equals(target, path, StringComparison.OrdinalIgnoreCase); // só mudou maiúscula/minúscula
        if (!sameFile && (File.Exists(target) || Directory.Exists(target)))
        {
            MessageBox.Show(this, $"Já existe \"{name}\" nesta pasta.", Product.Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            _documents.Rename(document, target);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show(this, $"Não foi possível renomear:\n{ex.Message}", Product.Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _settings.RecentFiles.RemoveAll(p => string.Equals(p, path, StringComparison.OrdinalIgnoreCase));
        RecentFiles.Add(_settings.RecentFiles, document.Path!);
        if (_folderPanel?.Contains(target) == true)
            _folderPanel.Reload();
    }
}
