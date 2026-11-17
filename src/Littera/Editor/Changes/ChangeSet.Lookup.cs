namespace Littera.Editor.Changes;

internal sealed partial record ChangeSet
{
    /// <summary>Trecho não salvo que aparece na linha; remoção conta na linha onde o texto sumiu.</summary>
    public DiffHunk? HunkAt(int line, int lineCount)
    {
        foreach (DiffHunk hunk in Unsaved)
        {
            int first = Math.Min(hunk.AfterStart, lineCount - 1);
            if (line >= first && line < first + Math.Max(1, hunk.AfterCount))
                return hunk;
        }

        return null;
    }

    /// <summary>Linha da versão salva → linha no editor; null se ela foi mexida de novo (está num trecho não salvo).</summary>
    private static int? ToCurrent(int savedLine, List<DiffHunk> unsaved)
    {
        int shift = 0;
        foreach (DiffHunk hunk in unsaved)
        {
            if (savedLine < hunk.BeforeStart)
                break;
            if (savedLine < hunk.BeforeStart + hunk.BeforeCount)
                return null;
            shift += hunk.AfterCount - hunk.BeforeCount;
        }

        return savedLine + shift;
    }
}
