using Littera.Services;

namespace Littera.UI;

/// <summary>Linha visível da árvore; Depth define o recuo.</summary>
internal readonly record struct FolderRow(FolderEntry Entry, int Depth);

internal sealed partial class FolderPanel
{
    private List<FolderRow> _rows = [];

    /// <summary>Pastas expandidas são lidas na hora; as recolhidas nem são abertas.</summary>
    private List<FolderRow> BuildTree()
    {
        var rows = new List<FolderRow>();
        AddChildren(rows, Folder!, 0);
        return rows;
    }

    private void AddChildren(List<FolderRow> rows, string folder, int depth)
    {
        foreach (FolderEntry entry in FolderScanner.Children(folder))
        {
            rows.Add(new FolderRow(entry, depth));
            if (entry.IsDirectory && _expanded.Contains(entry.Path))
                AddChildren(rows, entry.Path, depth + 1);
        }
    }

    private void ShowRows(List<FolderRow> rows)
    {
        _rows = rows;
        _hoverIndex = -1;
        _keyIndex = Filtering && rows.Count > 0 ? 0 : -1; // com filtro, Enter abre o primeiro resultado
        ClampScroll();
        Invalidate();
    }

    private void ToggleFolder(FolderRow row)
    {
        if (!_expanded.Remove(row.Entry.Path))
            _expanded.Add(row.Entry.Path);
        ShowRows(BuildTree());
    }

    private void CollapseAll()
    {
        _expanded.Clear();
        _scroll = 0;
        if (Folder is not null && string.IsNullOrWhiteSpace(_filter.Text))
            ShowRows(BuildTree());
    }
}
