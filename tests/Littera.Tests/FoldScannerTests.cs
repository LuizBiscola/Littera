using Littera.Editor.Folding;
using Littera.Editor.Highlights;

namespace Littera.Tests;

public class FoldScannerTests
{
    private const string Sql = """
        -- Relatório de vendas por cliente
        -- Autor: Luiz

        SET search_path TO vendas, public;

        WITH pedidos_recentes AS (
            SELECT p.id_pedido,
                   p.id_cliente
            FROM pedidos p
        )
        SELECT c.id_cliente,
               c.nome
        FROM clientes c
        WHERE c.ativo = TRUE;
        """;

    [Fact]
    public void Recolhe_comentarios_parenteses_e_o_resto_da_instrucao()
    {
        string text = Sql.Replace("\r\n", "\n");
        List<FoldRegion> regions = FoldScanner.Sql(text, BlockScanner.SqlStatements(text));
        Assert.Equal([new(0, 1), new(5, 9), new(10, 13)], regions.OrderBy(r => r.Header));
    }

    [Fact]
    public void Begin_e_case_recolhem_ate_o_end_mas_begin_tran_nao()
    {
        string text = "BEGIN TRAN;\nIF @x = 1\nBEGIN\n  SELECT CASE\n    WHEN a THEN 1\n  END;\nEND\nCOMMIT;";
        List<FoldRegion> regions = FoldScanner.Sql(text, []);
        Assert.Equal([new(2, 6), new(3, 5)], regions.OrderBy(r => r.Header));
    }

    [Fact]
    public void Niveis_marcam_cabecalhos_e_aninham_trechos()
    {
        int[] levels = FoldLevels.From(6, [new FoldRegion(0, 4), new FoldRegion(1, 3)]);
        const int b = FoldLevels.Base, h = FoldLevels.HeaderFlag;
        Assert.Equal([b | h, (b + 1) | h, b + 2, b + 2, b + 1, b], levels);
    }

    [Fact]
    public void Paragrafos_de_uma_linha_nao_recolhem()
    {
        Assert.Equal([new FoldRegion(2, 4)], FoldScanner.Paragraphs([new LineBlock(0, 0), new LineBlock(2, 4)]));
    }
}
