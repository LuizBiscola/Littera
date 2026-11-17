using Notas.Editor.Changes;

namespace Notas.Tests;

public class LineDiffTests
{
    private static readonly string[] Saved = ["SELECT", "  a,", "  b", "FROM t", "WHERE x = 1", ";"];

    [Fact]
    public void Versoes_iguais_nao_tem_trechos()
    {
        Assert.Empty(LineDiff.Hunks(Saved, [.. Saved])!);
    }

    [Fact]
    public void Linhas_trocadas_viram_um_trecho_com_removidas_e_novas()
    {
        string[] now = ["SELECT", "  COUNT(*),", "  SUM(v),", "  MAX(d)", "FROM t", "WHERE x = 1", ";"];
        Assert.Equal([new DiffHunk(1, 2, 1, 3)], LineDiff.Hunks(Saved, now)!);
    }

    [Fact]
    public void Insercao_e_remocao_separadas_viram_dois_trechos()
    {
        string[] now = ["-- novo", "SELECT", "  a,", "  b", "FROM t", ";"];
        Assert.Equal([new DiffHunk(0, 0, 0, 1), new DiffHunk(4, 1, 5, 0)], LineDiff.Hunks(Saved, now)!);
    }

    [Fact]
    public void Alteracoes_demais_desistem_de_comparar()
    {
        string[] now = [.. Enumerable.Range(0, 50).Select(i => $"linha {i}")];
        Assert.Null(LineDiff.Hunks(Saved, now, maxEdits: 10));
    }
}
