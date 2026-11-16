namespace Notas.UI;

internal sealed partial class MainForm
{
    /// <summary>"Abrir pasta…" (Ctrl+Shift+O): começa na pasta atual ou na do arquivo ativo.</summary>
    private void ShowOpenFolderDialog()
    {
        using var dialog = new FolderBrowserDialog { Description = "Abrir pasta", UseDescriptionForTitle = true };
        if ((_folderPanel?.Folder ?? Path.GetDirectoryName(_documents.Active?.Path)) is { } current)
            dialog.InitialDirectory = current;
        if (dialog.ShowDialog(this) == DialogResult.OK)
            OpenFolder(dialog.SelectedPath);
    }

    /// <summary>Botão da barra (Ctrl+Shift+E): sem pasta, pergunta qual abrir; com pasta, mostra ou oculta o painel.</summary>
    private void ToggleFolderPanel()
    {
        if (_folderPanel?.Folder is null)
            ShowOpenFolderDialog();
        else
            SetFolderShown(!_folderShown);
    }
}
