using Littera.Editor.Languages;

namespace Littera.Editor.Formatting;

internal sealed partial class SqlFormatter
{
    private static readonly HashSet<string> CasedWords = Words(SqlKeywords.Keywords, SqlKeywords.DataTypes, SqlKeywords.Functions);
    private static readonly HashSet<string> CallableWords = Words(SqlKeywords.DataTypes, SqlKeywords.Functions);

    /// <summary>Palavras que começam linha nova no recuo da cláusula.</summary>
    private static readonly HashSet<string> LineStarters = Words(
        "SELECT FROM WHERE HAVING LIMIT OFFSET VALUES RETURNING UNION EXCEPT INTERSECT WITH INSERT UPDATE DELETE MERGE " +
        "SET DECLARE PRINT EXEC EXECUTE IF ELSE RETURN CREATE ALTER DROP TRUNCATE WHILE FETCH USE GRANT REVOKE " +
        "GROUP ORDER JOIN INNER LEFT RIGHT FULL CROSS OUTER");

    private string Cased(string word) => !CasedWords.Contains(word) ? word : _options.KeywordCase switch
    {
        "lower" => word.ToLowerInvariant(),
        "keep" => word,
        _ => word.ToUpperInvariant(),
    };

    /// <summary>LEFT/GROUP/WITH... só abrem linha no papel de cláusula (não em LEFT(...), WITH (NOLOCK), DELETE FROM).</summary>
    private static bool StartsLine(string upper, SqlToken? previous, SqlToken? next)
    {
        if (!LineStarters.Contains(upper))
            return false;

        string before = previous?.Text.ToUpperInvariant() ?? "";
        return upper switch
        {
            "GROUP" or "ORDER" => next?.Is("BY") == true,
            "INNER" or "LEFT" or "RIGHT" or "FULL" or "CROSS" => next is { } n && (n.Is("JOIN") || n.Is("OUTER") || n.Is("APPLY")),
            "JOIN" or "OUTER" => before is not ("INNER" or "LEFT" or "RIGHT" or "FULL" or "CROSS" or "OUTER"),
            "WITH" => next?.Text != "(",
            "FROM" => before is not "DELETE",
            "CREATE" or "ALTER" or "DROP" => before is not "OR",
            _ => true,
        };
    }

    private static HashSet<string> Words(params string[] lists) =>
        new(lists.SelectMany(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries)), StringComparer.OrdinalIgnoreCase);
}
