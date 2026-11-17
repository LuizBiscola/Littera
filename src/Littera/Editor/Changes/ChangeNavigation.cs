namespace Littera.Editor.Changes;

/// <summary>Trechos alterados (linhas seguidas) e a navegação entre eles, dando a volta no arquivo.</summary>
internal static class ChangeNavigation
{
    /// <summary>Primeira e última linha de cada trecho alterado, de cima para baixo.</summary>
    public static List<(int First, int Last)> Runs(IEnumerable<int> lines)
    {
        var runs = new List<(int First, int Last)>();
        foreach (int line in lines)
        {
            if (runs.Count > 0 && runs[^1].Last == line - 1)
                runs[^1] = (runs[^1].First, line);
            else
                runs.Add((line, line));
        }

        return runs;
    }

    /// <summary>Começo do trecho seguinte (step &gt; 0) ou anterior ao da linha; -1 se não houver trechos.</summary>
    public static int Next(List<(int First, int Last)> runs, int line, int step)
    {
        if (runs.Count == 0)
            return -1;
        if (step > 0)
        {
            int next = runs.FindIndex(r => r.First > line);
            return runs[next >= 0 ? next : 0].First;
        }

        int previous = runs.FindLastIndex(r => r.Last < line && !(r.First <= line && line <= r.Last));
        return runs[previous >= 0 ? previous : runs.Count - 1].First;
    }
}
