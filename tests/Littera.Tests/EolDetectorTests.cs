using Littera.Services;

namespace Littera.Tests;

public class EolDetectorTests
{
    [Theory]
    [InlineData("a\r\nb\r\nc", nameof(LineEnding.CrLf))]
    [InlineData("a\nb\nc", nameof(LineEnding.Lf))]
    [InlineData("a\rb\rc", nameof(LineEnding.Cr))]
    [InlineData("a\r\nb\nc\nd", nameof(LineEnding.Lf))]   // misto: vence a maioria
    [InlineData("sem quebra", nameof(LineEnding.CrLf))]    // padrão do Windows
    [InlineData("", nameof(LineEnding.CrLf))]
    public void Escolhe_o_fim_de_linha_predominante(string text, string expected)
    {
        Assert.Equal(expected, EolDetector.Detect(text).ToString());
    }
}
