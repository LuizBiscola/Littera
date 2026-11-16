using Notas.Editor.Highlights;

namespace Notas.Editor.Folding;

internal static partial class FoldScanner
{
    /// <summary>Linha do "(" → linha do ")" que o fecha; com vários na mesma linha, vale o que fecha mais longe.</summary>
    private static Dictionary<int, int> Parentheses(string text)
    {
        var regions = new Dictionary<int, int>();
        var open = new Stack<int>();
        int line = 0, i = 0;
        while (i < text.Length)
        {
            char c = text[i];
            char next = i + 1 < text.Length ? text[i + 1] : '\0';
            if (c == '-' && next == '-') { i = SqlTextSkipper.ToEndOfLine(text, i); continue; }
            if (c == '/' && next == '*') { i = SqlTextSkipper.BlockComment(text, i, ref line); continue; }
            if (c is '\'' or '"' or '[') { i = SqlTextSkipper.Quoted(text, i, c == '[' ? ']' : c, ref line); continue; }
            if (c == '\n')
                line++;
            else if (c == '(')
                open.Push(line);
            else if (c == ')' && open.TryPop(out int from) && from < line)
                regions[from] = Math.Max(line, regions.GetValueOrDefault(from));
            i++;
        }

        return regions;
    }

    /// <summary>Duas ou mais linhas seguidas de comentário "--" (cabeçalhos de script, por exemplo).</summary>
    private static List<FoldRegion> CommentRuns(string text)
    {
        var runs = new List<FoldRegion>();
        int line = 0, first = -1;
        foreach (ReadOnlySpan<char> current in text.AsSpan().EnumerateLines())
        {
            bool comment = current.TrimStart().StartsWith("--");
            if (comment && first < 0)
                first = line;
            else if (!comment && first >= 0)
            {
                if (line - 1 > first)
                    runs.Add(new FoldRegion(first, line - 1));
                first = -1;
            }

            line++;
        }

        if (first >= 0 && line - 1 > first)
            runs.Add(new FoldRegion(first, line - 1));
        return runs;
    }
}
