namespace Notas.Editor.Changes;

internal static partial class LineDiff
{
    /// <summary>Remoções e inserções seguidas formam um trecho, em linhas do arquivo inteiro.</summary>
    private static List<DiffHunk> Group(List<Op> ops, int start)
    {
        var hunks = new List<DiffHunk>();
        int before = start, after = start, i = 0;
        while (i < ops.Count)
        {
            if (ops[i] == Op.Equal)
            {
                before++;
                after++;
                i++;
                continue;
            }

            int hunkBefore = before, hunkAfter = after;
            for (; i < ops.Count && ops[i] != Op.Equal; i++)
            {
                if (ops[i] == Op.Delete)
                    before++;
                else
                    after++;
            }

            hunks.Add(new DiffHunk(hunkBefore, before - hunkBefore, hunkAfter, after - hunkAfter));
        }

        return hunks;
    }
}
