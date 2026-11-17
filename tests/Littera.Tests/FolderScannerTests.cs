using Littera.Services;

namespace Littera.Tests;

public class FolderScannerTests
{
    private static string CreateTree()
    {
        string root = Directory.CreateTempSubdirectory().FullName;
        foreach (string dir in new[] { "views", "relatorios", "bin", ".git", "relatorios/mensal" })
            Directory.CreateDirectory(Path.Combine(root, dir));
        foreach (string file in new[] { "b.sql", "A.txt", "relatorios/vendas.sql", "relatorios/mensal/vendas_mes.sql", "bin/x.sql" })
            File.WriteAllText(Path.Combine(root, file), "");
        return root;
    }

    [Fact]
    public void Lista_pastas_antes_dos_arquivos_e_ignora_lixo()
    {
        string root = CreateTree();
        Assert.Equal(["relatorios", "views", "A.txt", "b.sql"], FolderScanner.Children(root).Select(e => e.Name));
    }

    [Fact]
    public void Filtro_busca_em_toda_a_arvore_fora_das_pastas_ignoradas()
    {
        string root = CreateTree();
        Assert.Equal(["vendas_mes.sql", "vendas.sql"], FolderScanner.Search(root, "VENDAS").Select(e => e.Name));
    }

    [Fact]
    public void Conta_arquivos_e_subpastas_e_avisa_quando_para_no_limite()
    {
        string root = CreateTree();
        Assert.Equal((4, 3, false), FolderScanner.Count(root));
        Assert.True(FolderScanner.Count(root, limit: 2).Partial);
    }
}
