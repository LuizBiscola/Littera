namespace Littera.UI.Find;

/// <summary>Ocorrências de um arquivo do escopo.</summary>
internal readonly record struct FileHits(FindFile File, int Count);

internal sealed partial class FindPanel
{
    private readonly System.Windows.Forms.Timer _filesTimer = new() { Interval = 250 };
    private List<FileHits> _fileHits = [];
    private CancellationTokenSource? _filesSearch;
    private bool _filesPending;

    /// <summary>Abas e pasta da janela, para os escopos com vários arquivos.</summary>
    public IFindFiles? Files { get; set; }

    private bool MultiFile => _scope is SearchScope.Tabs or SearchScope.Folder;

    /// <summary>Recontagem nos arquivos depois de uma pausa na digitação.</summary>
    private void ScheduleFiles()
    {
        _filesSearch?.Cancel();
        _filesTimer.Stop();
        _fileHits = [];
        _filesPending = MultiFile && Files is not null && _find.Text.Length > 0;
        if (_filesPending)
            _filesTimer.Start();
    }
}
