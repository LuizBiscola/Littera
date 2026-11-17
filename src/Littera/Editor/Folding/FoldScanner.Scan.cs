using Notas.Editor.Highlights;

namespace Notas.Editor.Folding;

internal static partial class FoldScanner
{
    /// <summary>
    /// Linha que abre → linha que fecha, para "(" … ")" e BEGIN/CASE … END de várias linhas;
    /// com vários na mesma linha, vale o que fecha mais longe.
    /// </summary>
    private static Dictionary<int, int> Brackets(string text)
    {
        var regions = new Dictionary<int, int>();
        var open = new Stack<(int Line, bool Word)>();
        int line = 0, i = 0;
        while (i < text.Length)
        {
            char c = text[i];
            char next = i + 1 < text.Length ? text[i + 1] : '\0';
            if (c == '-' && next == '-') { i = SqlTextSkipper.ToEndOfLine(text, i); continue; }
            if (c == '/' && next == '*') { i = SqlTextSkipper.BlockComment(text, i, ref line); continue; }
            if (c is '\'' or '"' or '[') { i = SqlTextSkipper.Quoted(text, i, c == '[' ? ']' : c, ref line); continue; }
            if (char.IsLetter(c) || c is '_' or '@' or '#')
            {
                int end = WordEnd(text, i);
                ReadOnlySpan<char> word = text.AsSpan(i, end - i);
                if ((IsWord(word, "BEGIN") && !IsTransaction(text, end)) || IsWord(word, "CASE"))
                    open.Push((line, true));
                else if (IsWord(word, "END") && open.TryPeek(out var block) && block.Word)
                    Close(regions, open.Pop().Line, line);
                i = end;
                continue;
            }

            if (c == '\n')
                line++;
            else if (c == '(')
                open.Push((line, false));
            else if (c == ')' && open.TryPeek(out var paren) && !paren.Word)
                Close(regions, open.Pop().Line, line);
            i++;
        }

        return regions;
    }

    private static void Close(Dictionary<int, int> regions, int from, int to)
    {
        if (from < to)
            regions[from] = Math.Max(to, regions.GetValueOrDefault(from));
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
