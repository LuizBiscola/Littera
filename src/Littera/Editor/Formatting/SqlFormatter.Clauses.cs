namespace Notas.Editor.Formatting;

internal sealed partial class SqlFormatter
{
    /// <summary>Palavra comum ou início de cláusula (abre linha no recuo do escopo).</summary>
    private void WriteWord(string upper, string word, SqlToken? previous, SqlToken? next)
    {
        if (StartsLine(upper, previous, next) && !(upper == "ELSE" && _blocks.TryPeek(out bool inBegin) && !inBegin))
        {
            NewLine(_base);
            _clause = upper;
            _hang = upper == "SELECT" && _options.OneColumnPerLine ? 7 : 0; // colunas alinhadas depois de "SELECT "
        }

        Write(word);
    }
}
