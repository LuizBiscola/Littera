namespace Notas.UI;

internal sealed partial class FolderPanel
{
    private bool _openFirst;

    /// <summary>Ctrl+P: foco no filtro para achar um arquivo pelo nome.</summary>
    public void FocusFilter()
    {
        _filter.Focus();
        _filter.SelectAll();
    }

    /// <summary>Enter no filtro: abre o primeiro arquivo encontrado, assim que a busca terminar.</summary>
    private void OpenFirstIfAsked()
    {
        if (!_openFirst)
            return;
        _openFirst = false;
        if (_rows.FirstOrDefault(r => !r.Entry.IsDirectory) is { Entry: { } entry })
            FileRequested?.Invoke(entry.Path);
    }
}
