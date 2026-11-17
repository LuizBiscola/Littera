namespace Littera.Editor.Changes;

/// <summary>Trecho diferente: BeforeCount linhas da versão salva viraram AfterCount linhas no editor.</summary>
internal readonly record struct DiffHunk(int BeforeStart, int BeforeCount, int AfterStart, int AfterCount);

/// <summary>
/// Diferença por linhas entre a versão salva e o editor (algoritmo de Myers). O começo e o fim iguais
/// são descartados antes, então o custo depende só do trecho entre a primeira e a última alteração.
/// </summary>
internal static partial class LineDiff
{
    private enum Op : byte
    {
        Equal,
        Delete,
        Insert,
    }

    /// <summary>null quando há alterações demais para comparar (arquivo praticamente reescrito).</summary>
    public static List<DiffHunk>? Hunks(IReadOnlyList<string> before, IReadOnlyList<string> after, int maxEdits = 1000)
    {
        int start = 0, endBefore = before.Count, endAfter = after.Count;
        while (start < endBefore && start < endAfter && before[start] == after[start])
            start++;
        while (endBefore > start && endAfter > start && before[endBefore - 1] == after[endAfter - 1])
        {
            endBefore--;
            endAfter--;
        }

        List<Op>? ops = Myers(before, after, start, endBefore, endAfter, maxEdits);
        return ops is null ? null : Group(ops, start);
    }
}
