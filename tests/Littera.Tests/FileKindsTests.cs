using Notas.Theme;

namespace Notas.Tests;

public class FileKindsTests
{
    public static TheoryData<string, string> Casos => new()
    {
        { @"C:\dados\vendas.sql", nameof(FileIcons.Database) },
        { "PRODUCAO.SQL", nameof(FileIcons.Database) },
        { "notas.txt", nameof(FileIcons.Text) },
        { "Makefile", nameof(FileIcons.Text) },
        { "deploy.ps1", nameof(FileIcons.Code) },
        { "config.jsonc", nameof(FileIcons.Braces) },
        { "pipeline.yml", nameof(FileIcons.Markup) },
        { "clientes.tsv", nameof(FileIcons.Sheet) },
        { ".env", nameof(FileIcons.Settings) },
        { "LEIA-ME.md", nameof(FileIcons.Document) },
        { "erro_carga.log", nameof(FileIcons.Log) },
        { "foto.xyz", nameof(FileIcons.File) },
    };

    [Theory]
    [MemberData(nameof(Casos))]
    public void Icone_vem_da_extensao(string path, string icon)
    {
        Assert.Same(typeof(FileIcons).GetField(icon)!.GetValue(null), FileKinds.For(path));
    }
}
