namespace Notas.Editor.Formatting;

internal sealed partial class SqlFormatter
{
    private bool _between; // o AND seguinte é do BETWEEN, não uma condição nova

    private void HandleWord(string text, SqlToken? previous, SqlToken? next)
    {
        string upper = text.ToUpperInvariant();
        string word = Cased(text);
        if (_inline > 0)
        {
            Write(word); // dentro de função, IN (...) ou OVER (...): tudo na mesma linha
            return;
        }

        switch (upper)
        {
            case "GO":
                NewLine(0);
                Write(word);
                BlankLine();
                return;
            case "BEGIN" when next is not { } n || !(n.Is("TRAN") || n.Is("TRANSACTION")):
                NewLine(_base);
                Write(word);
                _blocks.Push(true);
                _base++;
                _breakBefore = true;
                return;
            case "CASE":
                _blocks.Push(false);
                break;
            case "END" when _blocks.TryPop(out bool isBegin) && isBegin:
                _base = Math.Max(0, _base - 1);
                NewLine(_base);
                Write(word);
                return;
            case "BETWEEN":
                _between = true;
                break;
            case "AS" when _clause is "CREATE":
                NewLine(_base); // fim do cabeçalho de procedure/view: o corpo começa embaixo
                Write(word);
                _clause = null;
                return;
            case "AND" when _between:
                _between = false;
                break;
            case "AND" or "OR" when _options.BreakConditions && _clause is "WHERE" or "HAVING":
                NewLine(_base, Math.Max(0, _clause.Length - upper.Length)); // alinha à direita do WHERE, como no protótipo
                Write(word);
                return;
        }

        WriteWord(upper, word, previous, next);
    }
}
