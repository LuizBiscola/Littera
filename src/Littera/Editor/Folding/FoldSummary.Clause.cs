using Littera.Editor.Formatting;

namespace Littera.Editor.Folding;

internal static partial class FoldSummary
{
    private static readonly HashSet<string> ClauseEnds = new(StringComparer.OrdinalIgnoreCase)
    {
        "FROM", "INTO", "WHERE", "GROUP", "ORDER", "HAVING", "UNION", "EXCEPT", "INTERSECT", "LIMIT", "OFFSET",
        "SET", "VALUES", "RETURNING", "JOIN", "INNER", "LEFT", "RIGHT", "FULL", "CROSS", "SELECT", ";",
    };

    /// <summary>Tokens de primeiro nível até a próxima cláusula; o que está entre parênteses vira "(", ")".</summary>
    private static List<SqlToken> Clause(List<SqlToken> tokens, int at)
    {
        var list = new List<SqlToken>();
        int depth = 0;
        for (int i = at + 1; i < tokens.Count; i++)
        {
            SqlToken token = tokens[i];
            if (depth == 0 && ClauseEnds.Contains(token.Text))
                break;
            if (token.Is("("))
                depth++;
            if (depth == 0 || (depth == 1 && token.Is("(")))
                list.Add(token);
            if (token.Is(")") && --depth == 0)
                list.Add(token);
        }

        return list;
    }
}
