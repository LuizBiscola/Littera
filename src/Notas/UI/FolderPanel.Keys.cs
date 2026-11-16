using Notas.Theme;

namespace Notas.UI;

internal sealed partial class FolderPanel
{
    private bool _openFirst;
    private int _keyIndex = -1; // linha escolhida pelas setas no filtro

    /// <summary>Ctrl+P: foco no filtro para achar um arquivo pelo nome.</summary>
    public void FocusFilter()
    {
        _filter.Focus();
        _filter.SelectAll();
    }

    /// <summary>Setas no filtro escolhem a linha (rolando até ela); Enter abre.</summary>
    private void MoveKeySelection(int step)
    {
        if (_rows.Count == 0)
            return;
        _keyIndex = Math.Clamp(_keyIndex + step, 0, _rows.Count - 1);
        Rectangle row = RowBounds(_keyIndex);
        if (row.Top < TreeArea.Top)
            _scroll -= TreeArea.Top - row.Top + this.Px(2);
        else if (row.Bottom > TreeArea.Bottom)
            _scroll += row.Bottom - TreeArea.Bottom + this.Px(2);
        ClampScroll();
        Invalidate();
    }

    /// <summary>Enter: abre a linha escolhida; com a busca ainda atrasada, abre o primeiro arquivo quando ela terminar.</summary>
    private void OpenKeySelection()
    {
        if (_keyIndex >= 0 && _keyIndex < _rows.Count && !_filterTimer.Enabled)
        {
            FolderRow row = _rows[_keyIndex];
            if (row.Entry.IsDirectory)
                ToggleFolder(row);
            else
                FileRequested?.Invoke(row.Entry.Path);
        }
        else if (Filtering)
        {
            _openFirst = true;
            RefreshFilter();
        }
    }

    private void OpenFirstIfAsked()
    {
        if (!_openFirst)
            return;
        _openFirst = false;
        if (_rows.FirstOrDefault(r => !r.Entry.IsDirectory) is { Entry: { } entry })
            FileRequested?.Invoke(entry.Path);
    }
}
