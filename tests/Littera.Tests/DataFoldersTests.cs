using Littera.Services;

namespace Littera.Tests;

public sealed class DataFoldersTests : IDisposable
{
    private readonly string _root = Directory.CreateTempSubdirectory("littera-pastas-").FullName;

    public void Dispose() => Directory.Delete(_root, recursive: true);

    [Fact]
    public void Pasta_antiga_Notas_vira_Littera_com_o_conteudo()
    {
        Directory.CreateDirectory(Path.Combine(_root, "Notas"));
        File.WriteAllText(Path.Combine(_root, "Notas", "settings.json"), "{}");

        string folder = DataFolders.Prepare(_root);

        Assert.Equal(Path.Combine(_root, "Littera"), folder);
        Assert.True(File.Exists(Path.Combine(folder, "settings.json")));
        Assert.False(Directory.Exists(Path.Combine(_root, "Notas")));
    }

    [Fact]
    public void Com_a_pasta_nova_ja_existente_a_antiga_fica_como_esta()
    {
        Directory.CreateDirectory(Path.Combine(_root, "Notas"));
        Directory.CreateDirectory(Path.Combine(_root, "Littera"));

        Assert.Equal(Path.Combine(_root, "Littera"), DataFolders.Prepare(_root));
        Assert.True(Directory.Exists(Path.Combine(_root, "Notas")));
    }

    [Fact]
    public void Pasta_antiga_em_uso_continua_sendo_usada()
    {
        Directory.CreateDirectory(Path.Combine(_root, "Notas"));
        using var busy = new FileStream(Path.Combine(_root, "Notas", "session.json"), FileMode.Create, FileAccess.Write, FileShare.None);

        Assert.Equal(Path.Combine(_root, "Notas"), DataFolders.Prepare(_root));
    }
}
