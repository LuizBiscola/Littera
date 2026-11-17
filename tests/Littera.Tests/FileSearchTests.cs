using System.Text;
using Littera.Editor.Search;

namespace Littera.Tests;

public class FileSearchTests
{
    private const string Sql = "SELECT id_cliente, Id_Cliente FROM clientes WHERE id_cliente_antigo = 1;";

    [Theory]
    [InlineData("id_cliente", false, false, false, 3)]
    [InlineData("id_cliente", true, false, false, 2)]
    [InlineData("id_cliente", false, true, false, 2)]
    [InlineData(@"cliente\w*", false, false, true, 4)]
    [InlineData("id_cliente.", false, false, false, 0)] // sem regex o ponto é literal
    public void Conta_com_as_opcoes_do_painel(string text, bool matchCase, bool wholeWord, bool regex, int expected)
    {
        var search = FileSearch.ToRegex(new SearchQuery(text, matchCase, wholeWord, regex))!;
        Assert.Equal(expected, FileSearch.Count(Sql, search));
    }

    [Fact]
    public void Regex_invalida_ou_texto_vazio_nao_buscam()
    {
        Assert.Null(FileSearch.ToRegex(new SearchQuery("(", false, false, true)));
        Assert.Null(FileSearch.ToRegex(new SearchQuery("", false, false, false)));
    }

    [Fact]
    public void Le_texto_e_ignora_binarios()
    {
        string folder = Directory.CreateTempSubdirectory().FullName;
        string text = Path.Combine(folder, "a.sql"), binary = Path.Combine(folder, "b.bin");
        File.WriteAllText(text, "ação", new UTF8Encoding(true));
        File.WriteAllBytes(binary, [0x4D, 0x5A, 0x00, 0x01]);
        Assert.Equal("ação", FileSearch.ReadText(text));
        Assert.Null(FileSearch.ReadText(binary));
    }
}
