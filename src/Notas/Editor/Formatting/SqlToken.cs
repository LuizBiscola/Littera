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
