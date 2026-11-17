using System.Threading.Channels;

namespace Notas.Services;

/// <summary>Decodifica o arquivo numa thread de fundo e entrega o texto em blocos de ~256 mil caracteres.</summary>
internal static class TextChunkProducer
{
    public const int ChunkChars = 1 << 18;

    public static async Task ProduceAsync(Stream stream, TextEncodingKind kind, ChannelWriter<string> writer, CancellationToken ct)
    {
        try
        {
            // Com o encoding informado, o StreamReader pula o BOM correspondente sozinho.
            using var reader = new StreamReader(stream, kind.ToEncoding(), detectEncodingFromByteOrderMarks: false);
            char[] buffer = new char[ChunkChars];
            char carry = '\0';
            int read;
            while ((read = reader.ReadBlock(buffer, 0, buffer.Length)) > 0)
            {
                // Não corta um par substituto nem um CRLF entre dois blocos.
                char last = buffer[read - 1];
                int keep = read == buffer.Length && (char.IsHighSurrogate(last) || last == '\r') ? read - 1 : read;

                string chunk = carry == '\0'
                    ? new string(buffer, 0, keep)
                    : string.Concat(new ReadOnlySpan<char>(in carry), buffer.AsSpan(0, keep));
                carry = keep < read ? last : '\0';
                await writer.WriteAsync(chunk, ct);
            }

            if (carry != '\0')
                await writer.WriteAsync(carry.ToString(), ct);
            writer.Complete();
        }
        catch (Exception ex)
        {
            writer.Complete(ex); // quem lê os blocos recebe o erro (arquivo sumiu, sem permissão...)
        }
    }
}
