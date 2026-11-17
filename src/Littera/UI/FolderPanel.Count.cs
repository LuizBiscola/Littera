using Littera.Services;
using Littera.Theme;

namespace Littera.UI;

internal sealed partial class FolderPanel
{
    private string _countText = "";
    private CancellationTokenSource? _count;

    /// <summary>"13 arquivos · 4 pastas" do rodapé: percorre a árvore toda, então fica fora da thread da interface.</summary>
    private void CountInBackground()
    {
        _count?.Cancel();
        if (Folder is not null)
            WhenReady(() => _ = CountAsync());
    }

    private async Task CountAsync()
    {
        if (Folder is not { } folder)
            return;
        _count?.Cancel();
        var cancel = new CancellationTokenSource();
        _count = cancel;
        var (files, folders, partial) = await Task.Run(() => FolderScanner.Count(folder, cancel.Token));
        if (cancel.IsCancellationRequested || IsDisposed)
            return;

        string more = partial ? "+" : "";
        _countText = $"{files}{more} {Plural(files, "arquivo", "arquivos")} · {folders}{more} {Plural(folders, "pasta", "pastas")}";
        Invalidate(Rectangle.FromLTRB(0, Height - this.Px(FooterHeight), Width, Height));
    }

    /// <summary>Roda agora ou, com a pasta restaurada na abertura, quando a janela ficar pronta.</summary>
    private void WhenReady(Action action)
    {
        if (IsHandleCreated)
        {
            action();
            return;
        }

        void Run(object? sender, EventArgs e)
        {
            Application.Idle -= Run;
            if (!IsDisposed)
                action();
        }

        Application.Idle += Run;
    }

    private static string Plural(int count, string one, string many) => count == 1 ? one : many;
}
