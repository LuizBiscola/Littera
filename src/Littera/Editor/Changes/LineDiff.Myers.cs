namespace Notas.Editor.Changes;

internal static partial class LineDiff
{
    /// <summary>Menor sequência de edições entre before[start..endBefore) e after[start..endAfter), do começo para o fim.</summary>
    private static List<Op>? Myers(IReadOnlyList<string> before, IReadOnlyList<string> after, int start, int endBefore, int endAfter, int maxEdits)
    {
        int n = endBefore - start, m = endAfter - start;
        int max = Math.Min(n + m, maxEdits);
        int offset = max + 1;
        var v = new int[(2 * max) + 3];
        var trace = new List<int[]>();
        for (int d = 0; d <= max; d++)
        {
            trace.Add((int[])v.Clone());
            for (int k = -d; k <= d; k += 2)
            {
                int x = k == -d || (k != d && v[offset + k - 1] < v[offset + k + 1]) ? v[offset + k + 1] : v[offset + k - 1] + 1;
                int y = x - k;
                while (x < n && y < m && before[start + x] == after[start + y])
                {
                    x++;
                    y++;
                }

                v[offset + k] = x;
                if (x >= n && y >= m)
                    return Backtrack(trace, offset, n, m);
            }
        }

        return null;
    }
}
