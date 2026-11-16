namespace Notas.Editor.Formatting;

internal sealed partial class SqlFormatter
{
    /// <summary>Estado salvo ao entrar numa subconsulta "( SELECT ... )".</summary>
    private sealed record Scope(int Base, string? Clause, int Inline, int Hang);

    private readonly Stack<Scope> _scopes = new();
    private readonly Stack<bool> _blocks = new(); // true = BEGIN (recua), false = CASE (fica na linha)
    private int _base;        // recuo das cláusulas no escopo atual
    private string? _clause;  // SELECT, WHERE...
    private int _inline;      // parênteses em linha abertos: funções, IN (...), OVER (...)
    private int _hang;        // alinhamento das continuações (colunas do SELECT)

    private void OpenParen(bool subquery, bool spaceBefore)
    {
        Write("(", spaceBefore);
        if (!subquery)
        {
            _inline++;
            return;
        }

        _scopes.Push(new Scope(_base, _clause, _inline, _hang));
        (_base, _clause, _inline, _hang) = (_base + 1, null, 0, 0);
    }

    private void CloseParen()
    {
        if (_inline > 0 || _scopes.Count == 0)
        {
            _inline = Math.Max(0, _inline - 1);
            Write(")", spaceBefore: false);
            return;
        }

        Scope outer = _scopes.Pop();
        (_base, _clause, _inline, _hang) = (outer.Base, outer.Clause, outer.Inline, outer.Hang);
        NewLine(_base);
        Write(")", spaceBefore: false);
    }

}
