using Notas.Editor.Formatting;

namespace Notas.Tests;

public class SqlFormatterTests
{
    private static string Format(string sql, string keywordCase = "upper") =>
        SqlFormatter.Format(sql, new SqlFormatOptions(keywordCase, true, true, "    ", "\n"));

    [Fact]
    public void Formata_select_com_colunas_alinhadas_e_condicoes_em_rio()
    {
        string sql = "select c.id, c.nome, count(p.id) as qtd from clientes c inner join pedidos p on p.cli = c.id " +
                     "where c.ativo = 1 and p.valor between 10 and 20 group by c.id, c.nome order by qtd desc;";
        Assert.Equal(
            "SELECT c.id,\n       c.nome,\n       COUNT(p.id) AS qtd\nFROM clientes c\nINNER JOIN pedidos p ON p.cli = c.id\n" +
            "WHERE c.ativo = 1\n  AND p.valor BETWEEN 10 AND 20\nGROUP BY c.id, c.nome\nORDER BY qtd DESC;",
            Format(sql));
    }

    [Fact]
    public void Recua_subconsulta_e_fecha_parentese_em_linha_propria()
    {
        Assert.Equal("WITH x AS (\n    SELECT a\n    FROM t\n)\nSELECT *\nFROM x;", Format("with x as (select a from t) select * from x;"));
    }

    [Fact]
    public void Preserva_strings_comentarios_e_sinal_de_numero()
    {
        Assert.Equal("-- topo\nSELECT 'from; where',\n       [order],\n       DATEADD(DAY, -90, x) /* fim */\nFROM t;",
            Format("-- topo\nselect 'from; where', [order], dateadd(day, -90, x) /* fim */ from t;"));
    }

    [Fact]
    public void Separa_parametros_de_procedure_e_recua_begin_end()
    {
        string sql = "create procedure p @a int, @b int = 0 as begin set nocount on; select @a; end\ngo";
        Assert.Equal("CREATE PROCEDURE p\n    @a INT,\n    @b INT = 0\nAS\nBEGIN\n    SET NOCOUNT ON;\n    SELECT @a;\nEND\nGO", Format(sql));
    }

    [Fact]
    public void Respeita_caixa_escolhida_e_e_idempotente()
    {
        string once = Format("SELECT A FROM T WHERE X = 1;", "lower");
        Assert.Equal("select A\nfrom T\nwhere X = 1;", once);
        Assert.Equal(once, Format(once, "lower"));
    }
}
