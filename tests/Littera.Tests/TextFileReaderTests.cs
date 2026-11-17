using System.Text;
using Littera.Services;

namespace Littera.Tests;

public class TextFileReaderTests
{
    [Theory]
    [InlineData("\r\n")]           // CRLF bem na divisa entre dois blocos
    [InlineData("\U0001F600")]     // emoji = par substituto na divisa
    public async Task Nao_corta_sequencias_entre_blocos(string boundary)
    {
        string text = new string('a', TextChunkProducer.ChunkChars - 1) + boundary + "fim";
        List<string> chunks = await ReadAllChunks(text, new UTF8Encoding(false));

        Assert.Equal(text, string.Concat(chunks));
        Assert.All(chunks.SkipLast(1), c => Assert.False(c.EndsWith('\r') || char.IsHighSurrogate(c[^1])));
    }

    [Fact]
    public async Task Pula_o_bom_e_informa_o_encoding()
    {
        string path = Path.GetTempFileName();
        File.WriteAllText(path, "olá", new UTF8Encoding(true));
        TextFileReader reader = await TextFileReader.OpenAsync(path);

        Assert.Equal(TextEncodingKind.Utf8Bom.ToString(), reader.Encoding.ToString());
        Assert.Equal("olá", string.Concat(await reader.Chunks.ReadAllAsync().ToListAsync()));
    }

    private static async Task<List<string>> ReadAllChunks(string text, Encoding encoding)
    {
        string path = Path.GetTempFileName();
        await File.WriteAllTextAsync(path, text, encoding);
        TextFileReader reader = await TextFileReader.OpenAsync(path);
        return await reader.Chunks.ReadAllAsync().ToListAsync();
    }
}
