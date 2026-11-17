namespace Littera.Editor.Editing;

/// <summary>Alteração numa linha: tira Remove caracteres na coluna e põe Insert no lugar.</summary>
internal readonly record struct LineEdit(int Line, int Column, int Remove, string Insert);

/// <summary>
/// Ctrl+/: comenta as linhas no menor recuo do trecho ("-- " no SQL, "// " em C# e JS, "# " em Python);
/// se todas as linhas com texto já estão comentadas, descomenta. Linhas em branco não mudam.
/// </summary>
internal static class LineComments
{
    public static List<LineEdit> Toggle(IReadOnlyList<string> lines, string prefix)
    {
        List<int> code = [.. Enumerable.Range(0, lines.Count).Where(i => !string.IsNullOrWhiteSpace(lines[i]))];
        if (code.Count == 0)
            return [];

        if (code.TrueForAll(i => lines[i].AsSpan(Indent(lines[i])).StartsWith(prefix, StringComparison.Ordinal)))
            return [.. code.Select(i => Uncomment(lines[i], i, prefix))];

        int column = code.Min(i => Indent(lines[i]));
        return [.. code.Select(i => new LineEdit(i, column, 0, prefix + " "))];
    }

    /// <summary>Tira o prefixo e o espaço depois dele, se houver.</summary>
    private static LineEdit Uncomment(string line, int index, string prefix)
    {
        int at = Indent(line), after = at + prefix.Length;
        return new LineEdit(index, at, prefix.Length + (after < line.Length && line[after] == ' ' ? 1 : 0), "");
    }

    private static int Indent(string line) => line.Length - line.TrimStart().Length;
}
