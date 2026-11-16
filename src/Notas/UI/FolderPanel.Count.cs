using Notas.Services;
using Notas.Theme;

namespace Notas.UI;

internal sealed partial class FolderPanel
{
    private string _countText = "";
    private CancellationTokenSource? _count;

    /// <summary>"13 arquivos · 4 pastas" do rodapé: percorre a árvore toda, então fica fora da thread da interface.</summary>
    private void CountInBackground()
    {
        _count?.Cancel();
        Application.Idle -= CountWhenIdle;
        if (Folder is null)
            return;
        if (IsHandleCreated)
            _ = CountAsync();
        else
            Application.Idle += CountWhenIdle; // pasta restaurada na abertura: o rodapé espera a janela ficar pronta
    }

    private void CountWhenIdle(object? sender, EventArgs e)
    {
        Application.Idle -= CountWhenIdle;
        _ = CountAsync();
    }

    private async Task CountAsync()
    {
        if (Folder is not { } folder)
            return;
        var cancel = new CancellationTokenSource();
        _count = cancel;
        var (files, folders, partial) = await Task.Run(() => FolderScanner.Count(folder, cancel.Token));
        if (cancel.IsCancellationRequested || IsDisposed)
            return;

        string more = partial ? "+" : "";
        _countText = $"{files}{more} {Plural(files, "arquivo", "arquivos")} · {folders}{more} {Plural(folders, "pasta", "pastas")}";
        Invalidate(Rectangle.FromLTRB(0, Height - this.Px(FooterHeight), Width, Height));
    }

    private static string Plural(int count, string one, string many) => count == 1 ? one : many;
}
