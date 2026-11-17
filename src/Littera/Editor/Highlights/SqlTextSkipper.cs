namespace Notas.Editor.Highlights;

/// <summary>Pula comentários e literais do SQL contando as quebras de linha que atravessa.</summary>
internal static class SqlTextSkipper
{
    public static int ToEndOfLine(string text, int i)
    {
        int end = text.IndexOf('\n', i);
        return end < 0 ? text.Length : end;
    }

    /// <summary>/* ... */ (devolve a posição depois do fechamento).</summary>
    public static int BlockComment(string text, int i, ref int line)
    {
        int end = text.IndexOf("*/", i + 2, StringComparison.Ordinal);
        end = end < 0 ? text.Length : end + 2;
        line += CountLines(text, i, end);
        return end;
    }

    /// <summary>'texto', "identificador" ou [identificador]; aspas dobradas ('') continuam o literal.</summary>
    public static int Quoted(string text, int i, char close, ref int line)
    {
        int j = i + 1;
        while (j < text.Length)
        {
            if (text[j] == close)
            {
                if (close != ']' && j + 1 < text.Length && text[j + 1] == close)
                {
                    j += 2;
                    continue;
                }

                j++;
                break;
            }

            j++;
        }

        line += CountLines(text, i, j);
        return j;
    }

    private static int CountLines(string text, int start, int end) => text.AsSpan(start, end - start).Count('\n');
}
