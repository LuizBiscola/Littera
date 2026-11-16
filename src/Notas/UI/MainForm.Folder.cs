namespace Notas.UI;

internal sealed partial class MainForm
{
    private FolderPanel? _folderPanel;
    private bool _folderShown; // Visible é falso até a janela aparecer

    /// <summary>Criado só quando uma pasta é aberta: sem pasta, o painel não pesa na abertura.</summary>
    private FolderPanel FolderPanel
    {
        get
        {
            if (_folderPanel is null)
            {
                _folderPanel = new FolderPanel(_documents) { Visible = false };
                _folderPanel.FileRequested += OpenFile;
                _folderPanel.CloseRequested += CloseFolder;
                Controls.Add(_folderPanel);
                Controls.SetChildIndex(_folderPanel, 1); // encaixa depois da barra de status, como o painel de abas
            }

            return _folderPanel;
        }
    }

    private void OpenFolder(string folder, IEnumerable<string>? expanded = null)
    {
        FolderPanel.Open(folder, expanded);
        SetFolderShown(true);
    }

    private void CloseFolder()
    {
        SetFolderShown(false);
        _folderPanel?.CloseFolder();
    }

    private void SetFolderShown(bool shown)
    {
        _folderShown = shown;
        FolderPanel.Visible = shown && !_focusMode;
        _folderButton.Toggled = shown;
        TitleBar.Invalidate();
    }
}
