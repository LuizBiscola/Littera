using Notas.Editor.Highlights;

namespace Notas.Editor.Formatting;

/// <summary>Quebra o SQL em tokens preservando strings, identificadores entre aspas e comentários.</summary>
internal static class SqlTokenizer
{
    private static readonly string[] TwoCharSymbols = ["<>", "<=", ">=", "!=", "||", "::", "+=", "-="];

    public static List<SqlToken> Tokenize(string sql)
    {
        var tokens = new List<SqlToken>();
        int i = 0, ignored = 0;
        while (i < sql.Length)
        {
            char c = sql[i];
            char next = i + 1 < sql.Length ? sql[i + 1] : '\0';
            int start = i;
            SqlTokenKind kind;
            if (c == '\n') { i++; kind = SqlTokenKind.NewLine; }
            else if (char.IsWhiteSpace(c)) { i++; continue; }
            else if (c == '-' && next == '-') { i = SqlTextSkipper.ToEndOfLine(sql, i); kind = SqlTokenKind.LineComment; }
            else if (c == '/' && next == '*') { i = SqlTextSkipper.BlockComment(sql, i, ref ignored); kind = SqlTokenKind.BlockComment; }
            else if (c == '\'' || ((c is 'N' or 'n') && next == '\'')) { i = SqlTextSkipper.Quoted(sql, sql.IndexOf('\'', i), '\'', ref ignored); kind = SqlTokenKind.String; }
            else if (c is '"' or '[' or '`') { i = SqlTextSkipper.Quoted(sql, i, c == '[' ? ']' : c, ref ignored); kind = SqlTokenKind.Quoted; }
            else if (c == '@' || char.IsLetter(c) || c is '_' or '#') { i = WordEnd(sql, i + 1); kind = c == '@' ? SqlTokenKind.Variable : SqlTokenKind.Word; }
            else if (char.IsDigit(c) || (c == '.' && char.IsDigit(next))) { i = NumberEnd(sql, i + 1); kind = SqlTokenKind.Number; }
            else { i += TwoCharSymbols.Contains(sql.Substring(i, Math.Min(2, sql.Length - i))) ? 2 : 1; kind = SqlTokenKind.Symbol; }

            tokens.Add(new SqlToken(kind, sql[start..i].TrimEnd('\r')));
        }

        return tokens;
    }

    private static int WordEnd(string sql, int i)
    {
        while (i < sql.Length && (char.IsLetterOrDigit(sql[i]) || sql[i] is '_' or '$' or '#' or '@'))
            i++;
        return i;
    }

    private static int NumberEnd(string sql, int i)
    {
        while (i < sql.Length && (char.IsDigit(sql[i]) || sql[i] == '.' || ((sql[i] | 0x20) == 'e' && i + 1 < sql.Length && char.IsDigit(sql[i + 1]))))
            i++;
        return i;
    }
}
