namespace Littera.UI;

internal sealed partial class FolderPanel
{
    /// <summary>"Mostrar na pasta": abre as subpastas até o arquivo, rola até ele e o deixa destacado.</summary>
    public void ShowFile(string path)
    {
        if (!Contains(path))
            return;
        for (string? folder = Path.GetDirectoryName(path); folder is not null && Contains(folder + Path.DirectorySeparatorChar);
             folder = Path.GetDirectoryName(folder))
            _expanded.Add(folder);

        _filter.Text = "";
        _filterTimer.Stop();
        ShowRows(BuildTree());
        int index = _rows.FindIndex(r => string.Equals(r.Entry.Path, path, StringComparison.OrdinalIgnoreCase));
        if (index < 0)
            return;
        _keyIndex = index;
        MoveKeySelection(0); // rola até a linha
    }
}
