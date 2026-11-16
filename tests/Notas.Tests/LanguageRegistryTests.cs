using Notas.Editor.Languages;

namespace Notas.Tests;

public class LanguageRegistryTests
{
    [Theory]
    [InlineData(@"C:\x\vendas.sql", "SQL")]
    [InlineData(@"C:\x\VENDAS.SQL", "SQL")]
    [InlineData("dados.json", "JSON")]
    [InlineData("Notas.csproj", "XML")]
    [InlineData("Program.cs", "C#")]
    [InlineData("app.tsx", "JavaScript")]
    [InlineData("script.py", "Python")]
    [InlineData("README.md", "Markdown")]
    [InlineData("notas.txt", "Texto simples")]
    [InlineData("sem_extensao", "Texto simples")]
    [InlineData(null, "Texto simples")]
    public void Escolhe_pela_extensao(string? path, string expected)
    {
        Assert.Equal(expected, LanguageRegistry.ForPath(path).Name);
    }

    [Fact]
    public void Extensoes_nao_se_repetem_entre_linguagens()
    {
        string[] all = LanguageRegistry.All.SelectMany(l => l.Extensions).ToArray();
        Assert.Equal(all.Length, all.Distinct().Count());
    }
}
