using Littera.Editor.Folding;

namespace Littera.Tests;

public class FoldSummaryTests
{
    [Fact]
    public void Subconsulta_mostra_colunas_tabela_e_o_fechamento()
    {
        string[] hidden = ["    SELECT p.id_pedido,", "           p.id_cliente", "    FROM pedidos p", ")"];
        Assert.Equal("4 linhas │ SELECT 2 colunas · FROM pedidos  )", FoldSummary.For(hidden, sql: true));
    }

    [Fact]
    public void Update_mostra_coluna_e_condicao_unica_por_extenso()
    {
        string[] hidden = ["   SET segmento = 'VIP'", " WHERE id_cliente IN (SELECT id_cliente FROM vw_top_clientes);"];
        Assert.Equal("2 linhas │ SET segmento · WHERE id_cliente IN (…)", FoldSummary.For(hidden, sql: true));
    }

    [Fact]
    public void Varias_condicoes_viram_contagem_sem_contar_o_and_do_between()
    {
        string[] hidden = ["SELECT *", "FROM pedidos p", "JOIN itens i ON i.id = p.id", "WHERE p.total > 10 AND p.data BETWEEN 1 AND 2"];
        Assert.Equal("4 linhas │ SELECT * · FROM pedidos · 1 JOIN · WHERE 2 filtros", FoldSummary.For(hidden, sql: true));
    }

    [Fact]
    public void Texto_comum_mostra_a_primeira_linha()
    {
        Assert.Equal("2 linhas │ Primeira linha", FoldSummary.For(["  Primeira linha", "segunda"], sql: false));
    }
}
