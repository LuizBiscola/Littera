namespace Littera.Editor.Formatting;

internal sealed partial class SqlFormatter
{
    private void HandleSymbol(string symbol, SqlToken? previous, SqlToken? next)
    {
        switch (symbol)
        {
            case "(":
                bool subquery = next is { } n && (n.Is("SELECT") || n.Is("WITH"));
                OpenParen(subquery, spaceBefore: !IsCall(previous));
                break;
            case ")":
                CloseParen();
                break;
            case ",":
                Write(",", spaceBefore: false);
                if (_clause == "SELECT" && _inline == 0 && _options.OneColumnPerLine)
                    NewLine(_base, _hang);
                break;
            case ";":
                EndStatement();
                break;
            case "." or "::":
                Write(symbol, spaceBefore: false);
                break;
            default:
                Write(symbol);
                if (symbol is "-" or "+" && IsOperand(previous) is false)
                    _last = '('; // sinal de número ("-90"): o próximo token vem colado
                break;
        }
    }

    private static bool IsOperand(SqlToken? previous) => previous is { } p && (p.Text == ")"
        || p.Kind is SqlTokenKind.Number or SqlTokenKind.String or SqlTokenKind.Quoted or SqlTokenKind.Variable
        || (p.Kind == SqlTokenKind.Word && (!CasedWords.Contains(p.Text) || CallableWords.Contains(p.Text))));

    /// <summary>COUNT(, DECIMAL(, minha_funcao( ficam colados; IN (, VALUES (, AS ( levam espaço.</summary>
    private static bool IsCall(SqlToken? previous) => previous is { } p && p.Kind switch
    {
        SqlTokenKind.Word => CallableWords.Contains(p.Text) || !CasedWords.Contains(p.Text),
        SqlTokenKind.Quoted => true,
        _ => false,
    };
}
