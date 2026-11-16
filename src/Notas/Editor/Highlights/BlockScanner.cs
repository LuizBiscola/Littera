namespace Notas.Editor.Highlights;

/// <summary>Trecho do texto em linhas (base 0, inclusive).</summary>
internal readonly record struct LineBlock(int FirstLine, int LastLine)
{
    public bool Contains(int line) => line >= FirstLine && line <= LastLine;
}

/// <summary>Divide o texto nos blocos do destaque "bloco atual": instruções SQL ou parágrafos.</summary>
internal static class BlockScanner
{
    /// <summary>
    /// Instruções separadas por ';' ou por uma linha "GO". Comentários e linhas em branco
    /// nas pontas ficam de fora; ';' dentro de strings e comentários não conta.
    /// </summary>
    public static List<LineBlock> SqlStatements(string text)
    {
        var blocks = new List<LineBlock>();
        int line = 0, first = -1, last = -1, i = 0;
        bool lineHasCode = false;
        while (i < text.Length)
        {
            char c = text[i];
            char next = i + 1 < text.Length ? text[i + 1] : '\0';
            if (c == '\n') { line++; lineHasCode = false; i++; continue; }
            if (c == '-' && next == '-') { i = SqlTextSkipper.ToEndOfLine(text, i); continue; }
            if (c == '/' && next == '*') { i = SqlTextSkipper.BlockComment(text, i, ref line); continue; }
            if (char.IsWhiteSpace(c)) { i++; continue; }

            if (!lineHasCode && IsGoLine(text, i))
            {
                if (first >= 0) blocks.Add(new LineBlock(first, last));
                first = -1;
                i = SqlTextSkipper.ToEndOfLine(text, i);
                continue;
            }

            if (first < 0) first = line;
            last = line;
            lineHasCode = true;
            if (c is '\'' or '"' or '[') { i = SqlTextSkipper.Quoted(text, i, c == '[' ? ']' : c, ref line); last = line; continue; }
            if (c == ';') { blocks.Add(new LineBlock(first, last)); first = -1; }
            i++;
        }

        if (first >= 0)
            blocks.Add(new LineBlock(first, last));
        return blocks;
    }

    private static bool IsGoLine(string text, int i) =>
        i + 1 < text.Length && (text[i] | 0x20) == 'g' && (text[i + 1] | 0x20) == 'o'
        && text.AsSpan(i + 2, SqlTextSkipper.ToEndOfLine(text, i) - i - 2).IsWhiteSpace();
}
