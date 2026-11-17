using Notas.Services;

namespace Notas.Tests;

public class TextFileWriterTests
{
    [Theory]
    [InlineData(nameof(TextEncodingKind.Utf8), new byte[] { 0x6F, 0x6C, 0xC3, 0xA1 })]
    [InlineData(nameof(TextEncodingKind.Utf8Bom), new byte[] { 0xEF, 0xBB, 0xBF, 0x6F, 0x6C, 0xC3, 0xA1 })]
    [InlineData(nameof(TextEncodingKind.Ansi), new byte[] { 0x6F, 0x6C, 0xE1 })]
    public async Task Grava_no_encoding_escolhido(string encoding, byte[] expected)
    {
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
        await TextFileWriter.SaveAsync(path, "olá", Enum.Parse<TextEncodingKind>(encoding));
        Assert.Equal(expected, await File.ReadAllBytesAsync(path));
        File.Delete(path);
    }

    [Fact]
    public async Task Substitui_arquivo_existente_sem_deixar_temporario()
    {
        string folder = Directory.CreateTempSubdirectory().FullName;
        string path = Path.Combine(folder, "a.sql");
        await File.WriteAllTextAsync(path, "antigo");
        await TextFileWriter.SaveAsync(path, "novo", TextEncodingKind.Utf8);

        Assert.Equal("novo", await File.ReadAllTextAsync(path));
        Assert.Single(Directory.GetFiles(folder));
    }
}
