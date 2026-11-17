using Notas.Editor.Formatting;

namespace Notas.Editor.Folding;

internal static partial class FoldSummary
{
    /// <summary>Cláusulas de primeiro nível do trecho, resumidas; null se não houver nenhuma conhecida.</summary>
    private static string? SqlDetail(string sql)
    {
        List<SqlToken> tokens = [.. SqlTokenizer.Tokenize(sql).Where(t => t.IsSignificant)];
        var parts = new List<string>();
        int depth = 0, joins = 0;
        for (int i = 0; i < tokens.Count; i++)
        {
            SqlToken token = tokens[i];
            if (token.Is("("))
                depth++;
            else if (token.Is(")"))
                depth = Math.Max(0, depth - 1);
            if (depth > 0 || token.Kind != SqlTokenKind.Word)
                continue;

            string? part = token.Text.ToUpperInvariant() switch
            {
                "SELECT" => SelectPart(tokens, i),
                "FROM" when !parts.Exists(p => p.StartsWith("FROM ", StringComparison.Ordinal)) => "FROM " + NameAt(tokens, i + 1),
                "WHERE" or "HAVING" => ConditionPart(tokens, i),
                "UPDATE" => "UPDATE " + NameAt(tokens, i + 1),
                "INTO" when i > 0 && tokens[i - 1].Is("INSERT") => "INSERT INTO " + NameAt(tokens, i + 1),
                "SET" => SetPart(tokens, i),
                _ => null,
            };
            if (token.Is("JOIN"))
                joins++;
            if (part is not null)
                parts.Add(part);
        }

        int from = parts.FindIndex(p => p.StartsWith("FROM ", StringComparison.Ordinal));
        if (joins > 0 && from >= 0)
            parts.Insert(from + 1, joins == 1 ? "1 JOIN" : $"{joins} JOINs");
        return parts.Count == 0 ? null : Shorten(string.Join(" · ", parts), MaxDetail + 20);
    }
}
