using Littera.Services;

namespace Littera.Tests;

public class EncodingDetectorTests
{
    [Theory]
    [InlineData(new byte[] { 0xEF, 0xBB, 0xBF, 0x41 }, nameof(TextEncodingKind.Utf8Bom))]
    [InlineData(new byte[] { 0xFF, 0xFE, 0x41, 0x00 }, nameof(TextEncodingKind.Utf16LE))]
    [InlineData(new byte[] { 0xFE, 0xFF, 0x00, 0x41 }, nameof(TextEncodingKind.Utf16BE))]
    [InlineData(new byte[] { 0x41, 0x42 }, nameof(TextEncodingKind.Utf8))]
    [InlineData(new byte[] { 0xC3, 0xA7, 0xC3, 0xA3 }, nameof(TextEncodingKind.Utf8))] // "çã" em UTF-8
    [InlineData(new byte[] { 0x41, 0xE7, 0xE3 }, nameof(TextEncodingKind.Ansi))]       // "Açã" em Windows-1252
    [InlineData(new byte[0], nameof(TextEncodingKind.Utf8))]
    public void Detecta_pelo_bom_ou_pelo_conteudo(byte[] bytes, string expected)
    {
        using var stream = new MemoryStream(bytes);
        Assert.Equal(expected, EncodingDetector.Detect(stream).ToString());
        Assert.Equal(0, stream.Position);
    }

    [Fact]
    public void Byte_ansi_depois_de_um_mega_vira_ansi()
    {
        byte[] bytes = new byte[(1 << 20) + 10];
        Array.Fill(bytes, (byte)'a');
        bytes[^1] = 0xE9; // "é" em Windows-1252, sozinho no fim
        Assert.Equal(TextEncodingKind.Ansi, EncodingDetector.Detect(new MemoryStream(bytes)));
    }

    [Fact]
    public void Caractere_multibyte_cortado_entre_blocos_continua_utf8()
    {
        byte[] bytes = new byte[(1 << 20) + 4];
        Array.Fill(bytes, (byte)'a');
        bytes[(1 << 20) + 3 - 1] = 0xC3; // "ã" começa no último byte do primeiro bloco...
        bytes[(1 << 20) + 3] = 0xA3;     // ...e termina no começo do segundo
        Assert.Equal(TextEncodingKind.Utf8, EncodingDetector.Detect(new MemoryStream(bytes)));
    }
}
