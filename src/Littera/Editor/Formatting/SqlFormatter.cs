namespace Littera.Editor.Formatting;

/// <summary>
/// Formatador de SQL: palavras-chave no caso escolhido, cláusulas em linhas próprias, colunas do SELECT
/// alinhadas, AND/OR alinhados à direita do WHERE, subconsultas e BEGIN...END recuados.
/// Strings, identificadores entre aspas e comentários passam intactos.
/// </summary>
internal sealed partial class SqlFormatter
{
    public static string Format(string sql, SqlFormatOptions options)
    {
        var formatter = new SqlFormatter(options);
        formatter.Run(SqlTokenizer.Tokenize(sql));
        string result = formatter._out.ToString().TrimEnd();
        return sql.EndsWith('\n') ? result + options.NewLine : result;
    }

    private void Run(List<SqlToken> tokens)
    {
        for (int i = 0; i < tokens.Count; i++)
        {
            SqlToken token = tokens[i];
            if (token.Kind == SqlTokenKind.NewLine)
            {
                // Linha em branco do original só vale entre instruções.
                if (_clause is null && _scopes.Count == 0 && i > 0 && tokens[i - 1].Kind == SqlTokenKind.NewLine)
                    BlankLine();
                continue;
            }

            BreakIfPending();
            SqlToken? previous = SqlTokens.Significant(tokens, i, -1);
            SqlToken? next = SqlTokens.Significant(tokens, i, +1);
            switch (token.Kind)
            {
                case SqlTokenKind.LineComment:
                    Write(token.Text);
                    NewLine(_base, _hang);
                    break;
                case SqlTokenKind.Word:
                    HandleWord(token.Text, previous, next);
                    break;
                case SqlTokenKind.Symbol:
                    HandleSymbol(token.Text, previous, next);
                    break;
                case SqlTokenKind.Variable when _clause is "CREATE" && _inline == 0 && previous is { } p && p.Kind != SqlTokenKind.Symbol | p.Text == ",":
                    NewLine(_base + 1); // parâmetros da procedure, um por linha
                    Write(token.Text);
                    break;
                default:
                    Write(token.Text);
                    break;
            }
        }
    }

}
