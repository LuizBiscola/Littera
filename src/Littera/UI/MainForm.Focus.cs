namespace Littera.UI;

internal sealed partial class MainForm
{
    private bool _focusMode;

    /// <summary>F11: só o texto (sem barra de título, status e abas). F11 ou Esc voltam.</summary>
    private void ToggleFocusMode()
    {
        _focusMode = !_focusMode;
        SuspendLayout();
        TitleBar.Visible = !_focusMode;
        _status.Visible = !_focusMode;
        _openFiles.Visible = !_focusMode && _settings.Window.ShowOpenFiles;
        if (_folderPanel is not null)
            _folderPanel.Visible = !_focusMode && _folderShown;
        ResumeLayout(performLayout: true);
        _documents.Active?.Editor?.Focus();
    }

    /// <summary>Esc fecha primeiro a busca, depois sai do modo foco.</summary>
    private bool HandleEscape()
    {
        if (_palette?.Visible == true)
        {
            _palette.Dismiss();
            _boundEditor?.Focus();
            return true;
        }

        if (_changePreview?.Visible == true)
        {
            _changePreview.Dismiss();
            return true;
        }

        if (_foldPreview?.Visible == true)
        {
            _foldPreview.Dismiss();
            return true;
        }

        if (_findPanel?.Visible == true)
        {
            _findPanel.Dismiss();
            return true;
        }

        if (!_focusMode)
            return false;
        ToggleFocusMode();
        return true;
    }
}
