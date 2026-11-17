using Littera.Editor.Changes;

namespace Littera.Tests;

public class ChangeSetTests
{
    private static readonly string[] Disk = ["SELECT", "  a,", "  b", "FROM t", ""];

    [Fact]
    public void Digitar_e_apagar_nao_deixa_marca()
    {
        ChangeSet set = ChangeSet.Compute(Disk, Disk, [.. Disk])!;
        Assert.Empty(set.UnsavedLines);
        Assert.Empty(set.SavedLines);
    }

    [Fact]
    public void Espaco_a_mais_conta_como_alteracao()
    {
        string[] now = ["SELECT", "  a, ", "  b", "FROM t", ""];
        Assert.Equal([1], ChangeSet.Compute(Disk, Disk, now)!.UnsavedLines);
    }

    [Fact]
    public void Linha_em_branco_nova_e_remocao_marcam_onde_aconteceram()
    {
        string[] now = ["SELECT", "", "  a,", "FROM t", ""];
        ChangeSet set = ChangeSet.Compute(Disk, Disk, now)!;
        Assert.Equal([1, 3], set.UnsavedLines);
        Assert.NotNull(set.HunkAt(3, now.Length));
    }

    [Fact]
    public void Salvas_acompanham_linhas_inseridas_acima_depois_de_salvar()
    {
        string[] saved = ["SELECT", "  a,", "  c", "FROM t", ""];
        string[] now = ["-- novo", "SELECT", "  a,", "  c", "FROM t", ""];
        ChangeSet set = ChangeSet.Compute(Disk, saved, now)!;
        Assert.Equal([0], set.UnsavedLines);
        Assert.Equal([3], set.SavedLines);
    }
}
