using Notas.Editor.Highlights;

namespace Notas.Tests;

public class BlockScannerTests
{
    [Fact]
    public void Instrucoes_do_prototipo_ignoram_comentarios_nas_pontas()
    {
        string sql = "-- relatório\n\nSET a TO b;\n\nWITH x AS (\n  SELECT ';' AS c -- ; aqui não\n)\nSELECT 1;\n/* fim; */\nUPDATE t\n SET v = 1\n WHERE id IN (SELECT 1);";
        Assert.Equal([new(2, 2), new(4, 7), new(9, 11)], BlockScanner.SqlStatements(sql));
    }

    [Fact]
    public void Linha_GO_separa_lotes_do_TSQL()
    {
        string sql = "CREATE PROCEDURE p AS\nBEGIN\n  SELECT 1\nEND\nGO\nEXEC p\ngo";
        Assert.Equal([new(0, 3), new(5, 5)], BlockScanner.SqlStatements(sql));
    }

    [Fact]
    public void Colchetes_e_aspas_dobradas_nao_quebram_a_instrucao()
    {
        string sql = "SELECT [a;b], 'it''s; ok'\nFROM t;";
        Assert.Equal([new(0, 1)], BlockScanner.SqlStatements(sql));
    }

    [Fact]
    public void Paragrafos_sao_linhas_seguidas_nao_vazias()
    {
        string text = "a\nb\n\n  \nc\r\nd\r\n\r\ne";
        Assert.Equal([new(0, 1), new(4, 5), new(7, 7)], BlockScanner.Paragraphs(text));
    }
}
