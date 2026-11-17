using Notas.Editor.Formatting;

namespace Notas.Tests;

public class SqlTokenizerTests
{
    private static string[] Texts(string sql) => SqlTokenizer.Tokenize(sql).Select(t => t.Text).ToArray();

    [Fact]
    public void Separa_palavras_simbolos_e_numeros()
    {
        Assert.Equal(["SELECT", "a", ".", "b", ",", "COUNT", "(", "*", ")", "FROM", "t", "WHERE", "x", ">=", "1.5"],
            Texts("SELECT a.b, COUNT(*) FROM t WHERE x >= 1.5"));
    }

    [Fact]
    public void Mantem_strings_identificadores_e_variaveis_inteiros()
    {
        Assert.Equal(["'it''s; --'", "N'olá'", "[a b]", "\"c d\"", "@total", "@@ROWCOUNT", "#temp"],
            Texts("'it''s; --' N'olá' [a b] \"c d\" @total @@ROWCOUNT #temp"));
    }

    [Fact]
    public void Comentarios_e_quebras_viram_tokens_proprios()
    {
        List<SqlToken> tokens = SqlTokenizer.Tokenize("a -- fim\r\n/* bloco */ b");
        Assert.Equal(["a", "-- fim", "\n", "/* bloco */", "b"], tokens.Select(t => t.Text));
        Assert.Equal(nameof(SqlTokenKind.LineComment), tokens[1].Kind.ToString());
    }
}
