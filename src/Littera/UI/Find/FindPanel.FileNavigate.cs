using Littera.Editor.Search;
using ScintillaNET;

namespace Littera.UI.Find;

internal sealed partial class FindPanel
{
    private int ActiveHitIndex => Files?.Active is { } active ? _fileHits.FindIndex(h => h.File.Key == active.Key) : -1;

    /// <summary>Ocorrência que a navegação quer no arquivo ativo; fora de 0..Count-1 significa outro arquivo.</summary>
    private int TargetIndex(Scintilla editor, int step)
    {
        if (editor.SelectionStart != editor.SelectionEnd)
            return _session.Current + step; // a seleção já é uma ocorrência
        int next = _session.AfterLast ? _session.Count : _session.Current;
        return step < 0 ? next - 1 : next;
    }

    /// <summary>Passou da primeira ou da última ocorrência do arquivo: abre o próximo arquivo do escopo com resultados.</summary>
    private bool StepToFile(int step)
    {
        int start = ActiveHitIndex, count = _fileHits.Count;
        int origin = start >= 0 ? start : step > 0 ? -1 : count;
        for (int n = 1; n <= count; n++)
        {
            int i = ((origin + step * n) % count + count) % count;
            if (i != start && _fileHits[i].Count > 0)
            {
                _ = ShowFileAsync(_fileHits[i].File, step);
                return true;
            }
        }

        return false; // só o arquivo ativo tem resultados: dá a volta nele mesmo
    }

    private async Task ShowFileAsync(FindFile file, int step)
    {
        if (await Files!.ShowAsync(file) is not { } editor)
            return;
        _session.Run(editor, Query, new TextRange(0, editor.TextLength), 0);
        _session.Select(editor, step > 0 ? 0 : -1);
        Invalidate();
    }

    /// <summary>"3 de 17" contando as ocorrências dos arquivos anteriores ao ativo.</summary>
    private string FilesLabel()
    {
        if (_filesPending)
            return _session.Count == 0 ? "Buscando…" : $"{_session.Label}…";
        int active = ActiveHitIndex;
        int total = _fileHits.Where((h, i) => i != active).Sum(h => h.Count) + _session.Count;
        if (total == 0)
            return "Nenhum resultado";
        if (_session.Count == 0 || active < 0)
            return $"– de {total}";
        return $"{_fileHits.Take(active).Sum(h => h.Count) + _session.Current + 1} de {total}";
    }
}
