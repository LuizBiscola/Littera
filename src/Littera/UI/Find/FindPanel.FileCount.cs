using Littera.Editor.Search;

namespace Littera.UI.Find;

internal sealed partial class FindPanel
{
    private async Task CountFilesAsync()
    {
        _filesTimer.Stop();
        if (Files is not { } files || FileSearch.ToRegex(Query) is not { } regex)
        {
            _filesPending = false;
            Invalidate();
            return;
        }

        var cancel = new CancellationTokenSource();
        _filesSearch = cancel;
        SearchQuery query = Query;
        List<FindFile> list = await files.ListAsync(_scope);
        if (cancel.IsCancellationRequested)
            return;

        // Abas carregadas: o próprio editor conta (sem copiar o texto); o resto é lido do disco em segundo plano.
        int?[] counts = [.. list.Select(f => files.LoadedEditor(f) is { } editor
            ? TextSearch.FindAll(editor, query, new TextRange(0, editor.TextLength)).Count : (int?)null)];
        await Task.Run(() =>
        {
            for (int i = 0; i < list.Count && !cancel.IsCancellationRequested; i++)
                counts[i] ??= list[i].Path is { } path && FileSearch.ReadText(path) is { } text ? FileSearch.Count(text, regex) : 0;
        });
        if (cancel.IsCancellationRequested || IsDisposed)
            return;

        _fileHits = [.. list.Select((file, i) => new FileHits(file, counts[i] ?? 0))];
        _filesPending = false;
        Invalidate();
    }
}
