namespace Littera.UI;

internal sealed partial class FolderPanel
{
    public void Open(string folder, IEnumerable<string>? expanded = null)
    {
        Folder = Path.TrimEndingDirectorySeparator(Path.GetFullPath(folder));
        _expanded.Clear();
        _expanded.UnionWith(expanded ?? []);
        _scroll = 0;
        _filter.Text = "";
        _filterTimer.Stop(); // limpar o texto agendaria outra leitura
        Reload();
        WhenReady(Watch);
    }

    /// <summary>Relê a pasta do disco (botão atualizar ou arquivo salvo dentro dela).</summary>
    public void Reload()
    {
        if (Folder is null)
            return;
        if (Filtering)
            RefreshFilter();
        else
            ShowRows(BuildTree());
        CountInBackground();
    }

    /// <summary>"Fechar pasta": esquece a pasta; quem chamou esconde o painel.</summary>
    public void CloseFolder()
    {
        _search?.Cancel();
        _count?.Cancel();
        StopWatching();
        Folder = null;
        _expanded.Clear();
        _filter.Text = "";
        _filterTimer.Stop();
        _rows = [];
        _countText = "";
    }

    /// <summary>O arquivo está dentro da pasta aberta (em qualquer nível)?</summary>
    public bool Contains(string path) =>
        Folder is not null && path.StartsWith(Folder + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
}
