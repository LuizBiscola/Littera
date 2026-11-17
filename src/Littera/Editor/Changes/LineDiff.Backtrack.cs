namespace Notas.Editor.Changes;

internal static partial class LineDiff
{
    /// <summary>Refaz o caminho de trás para frente usando o estado guardado de cada rodada.</summary>
    private static List<Op> Backtrack(List<int[]> trace, int offset, int n, int m)
    {
        var ops = new List<Op>();
        int x = n, y = m;
        for (int d = trace.Count - 1; d >= 0; d--)
        {
            int[] v = trace[d];
            int k = x - y;
            int previousK = k == -d || (k != d && v[offset + k - 1] < v[offset + k + 1]) ? k + 1 : k - 1;
            int previousX = v[offset + previousK], previousY = previousX - previousK;
            while (x > previousX && y > previousY)
            {
                ops.Add(Op.Equal);
                x--;
                y--;
            }

            if (d > 0)
                ops.Add(x == previousX ? Op.Insert : Op.Delete);
            x = previousX;
            y = previousY;
        }

        ops.Reverse();
        return ops;
    }
}
