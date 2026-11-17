namespace Notas.Editor.Formatting;

internal enum SqlTokenKind
{
    Word,
    Variable,
    Quoted,
    String,
    Number,
    LineComment,
    BlockComment,
    Symbol,
    NewLine,
}

/// <summary>Pedaço do SQL; espaços não viram token (o formatador refaz o espaçamento).</summary>
internal readonly record struct SqlToken(SqlTokenKind Kind, string Text)
{
    public bool Is(string text) => string.Equals(Text, text, StringComparison.OrdinalIgnoreCase);

    public bool IsSignificant => Kind is not (SqlTokenKind.NewLine or SqlTokenKind.LineComment or SqlTokenKind.BlockComment);
}

internal static class SqlTokens
{
    /// <summary>Próximo token que não é quebra nem comentário, para frente (step = 1) ou para trás (-1).</summary>
    public static SqlToken? Significant(List<SqlToken> tokens, int from, int step)
    {
        for (int i = from + step; i >= 0 && i < tokens.Count; i += step)
        {
            if (tokens[i].IsSignificant)
                return tokens[i];
        }

        return null;
    }
}
