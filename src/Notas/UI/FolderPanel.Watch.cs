namespace Notas.UI;

internal sealed partial class FolderPanel
{
    private readonly System.Windows.Forms.Timer _watchTimer = new() { Interval = 400 };
    private FileSystemWatcher? _watcher;

    /// <summary>Arquivos criados, apagados ou renomeados fora do Notas aparecem sozinhos, depois de uma pausa.</summary>
    private void Watch()
    {
        StopWatching();
        if (Folder is not { } folder)
            return;
        try
        {
            _watcher = new FileSystemWatcher(folder)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName,
            };
            _watcher.Created += OnFolderChanged;
            _watcher.Deleted += OnFolderChanged;
            _watcher.Renamed += OnFolderChanged;
            _watcher.EnableRaisingEvents = true;
        }
        catch (Exception ex) when (ex is ArgumentException or IOException or PlatformNotSupportedException)
        {
            StopWatching(); // pasta de rede ou sem permissão: fica o botão "Atualizar"
        }
    }

    private void StopWatching()
    {
        _watcher?.Dispose();
        _watcher = null;
        _watchTimer.Stop();
    }

    /// <summary>Chega numa thread do sistema: só reinicia a espera na thread da interface.</summary>
    private void OnFolderChanged(object sender, FileSystemEventArgs e)
    {
        if (IsHandleCreated && !IsDisposed)
            BeginInvoke(() =>
            {
                _watchTimer.Stop();
                _watchTimer.Start();
            });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            StopWatching();
        base.Dispose(disposing);
    }
}
