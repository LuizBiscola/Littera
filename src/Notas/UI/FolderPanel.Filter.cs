using Notas.Services;
using Notas.Theme;

namespace Notas.UI;

internal sealed partial class FolderPanel
{
    private readonly TextBox _filter = new()
    {
        BorderStyle = BorderStyle.None,
        BackColor = Palette.Background,
        ForeColor = Palette.Text,
        PlaceholderText = "Filtrar arquivos…",
    };

    private readonly System.Windows.Forms.Timer _filterTimer = new() { Interval = 150 };
    private CancellationTokenSource? _search;

    /// <summary>Com filtro, a árvore vira uma lista dos arquivos encontrados em todas as subpastas.</summary>
    private bool Filtering => !string.IsNullOrWhiteSpace(_filter.Text);

    private void CreateFilter()
    {
        Controls.Add(_filter);
        _filter.GotFocus += (_, _) => Invalidate(FilterBox); // borda azul com o foco
        _filter.LostFocus += (_, _) => Invalidate(FilterBox);
        _filterTimer.Tick += (_, _) => RefreshFilter();
        _filter.TextChanged += (_, _) =>
        {
            _filterTimer.Stop();
            _filterTimer.Start(); // espera a digitação parar
        };
        _filter.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Escape)
                _filter.Clear();
            else if (e.KeyCode is Keys.Down or Keys.Up)
                MoveKeySelection(e.KeyCode == Keys.Down ? 1 : -1);
            else if (e.KeyCode == Keys.Enter)
                OpenKeySelection();
            else
                return;
            e.SuppressKeyPress = true;
        };
    }

    /// <summary>Busca em segundo plano: pastas grandes não travam a digitação.</summary>
    private async void RefreshFilter()
    {
        _filterTimer.Stop();
        _search?.Cancel();
        if (Folder is not { } folder || !Filtering)
        {
            if (Folder is not null)
                ShowRows(BuildTree());
            return;
        }

        var cancel = new CancellationTokenSource();
        _search = cancel;
        string text = _filter.Text.Trim();
        List<FolderEntry> found = await Task.Run(() => FolderScanner.Search(folder, text, cancel.Token));
        if (cancel.IsCancellationRequested || IsDisposed)
            return;
        ShowRows([.. found.Select(entry => new FolderRow(entry, 0))]);
        OpenFirstIfAsked();
    }
}
