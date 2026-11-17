using Notas.Editor.Search;

namespace Notas.UI.Find;

internal sealed partial class FindPanel
{
    /// <summary>"Todas" com vários arquivos: abre cada um com resultados e substitui. Nada é salvo: fica para revisar.</summary>
    private async Task ReplaceInFilesAsync()
    {
        if (Files is not { } files)
            return;
        if (_filesPending)
            await CountFilesAsync();

        FindFile? original = files.Active;
        SearchQuery query = Query;
        string replacement = _replace!.Text;
        int total = 0, changed = 0;
        foreach (FileHits hit in _fileHits.Where(h => h.Count > 0).ToList())
        {
            if (await files.ShowAsync(hit.File) is not { } editor)
                continue;
            int count = TextSearch.ReplaceAll(editor, query, replacement, new TextRange(0, editor.TextLength));
            total += count;
            changed += count > 0 ? 1 : 0;
        }

        if (original is not null)
            await files.ShowAsync(original);
        Search(select: false);
        _toolTip ??= new ToolTip();
        string where = changed == 1 ? "1 arquivo" : $"{changed} arquivos";
        _toolTip.Show($"{total} substituições em {where}, ainda não salvos", this, _replaceAll!.Bounds.X, Height, 3000);
    }
}
