using System.Threading.Channels;

namespace Littera.Services;

/// <summary>Arquivo aberto para leitura: encoding detectado e o texto chegando em blocos.</summary>
internal sealed class TextFileReader
{
    private TextFileReader(TextEncodingKind encoding, ChannelReader<string> chunks)
    {
        Encoding = encoding;
        Chunks = chunks;
    }

    public TextEncodingKind Encoding { get; }

    /// <summary>Erros de leitura aparecem ao consumir os blocos.</summary>
    public ChannelReader<string> Chunks { get; }

    public static async Task<TextFileReader> OpenAsync(string path, CancellationToken ct = default)
    {
        var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1 << 16, FileOptions.SequentialScan);
        try
        {
            TextEncodingKind kind = await Task.Run(() => EncodingDetector.Detect(stream), ct);
            var channel = Channel.CreateBounded<string>(2);
            _ = Task.Run(() => TextChunkProducer.ProduceAsync(stream, kind, channel.Writer, ct), ct);
            return new TextFileReader(kind, channel.Reader);
        }
        catch
        {
            await stream.DisposeAsync();
            throw;
        }
    }
}
