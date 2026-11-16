using System.Buffers;
using System.Text.Unicode;

namespace Notas.Services;

/// <summary>
/// Detecta o encoding: BOM primeiro; sem BOM, UTF-8 se o arquivo inteiro for UTF-8 válido, senão ANSI.
/// Valida o arquivo todo (vetorizado, poucos ms para 50 MB) para não errar com um byte ANSI perdido no fim.
/// </summary>
internal static class EncodingDetector
{
    private static bool IsUtf8(Stream stream)
    {
        byte[] buffer = ArrayPool<byte>.Shared.Rent((1 << 20) + 4);
        try
        {
            int carry = 0, read;
            while ((read = stream.Read(buffer, carry, buffer.Length - carry)) > 0)
            {
                int total = carry + read;
                int complete = CompletePrefix(buffer.AsSpan(0, total));
                if (!Utf8.IsValid(buffer.AsSpan(0, complete)))
                    return false;

                carry = total - complete;
                buffer.AsSpan(complete, carry).CopyTo(buffer);
            }

            return carry == 0;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    /// <summary>Tamanho do bloco sem a sequência multibyte cortada no fim (até 3 bytes).</summary>
    private static int CompletePrefix(ReadOnlySpan<byte> data)
    {
        for (int i = 1; i <= Math.Min(3, data.Length); i++)
        {
            byte b = data[^i];
            if ((b & 0xC0) == 0x80)
                continue; // byte de continuação: olha o anterior
            if (b < 0xC0)
                return data.Length; // ASCII: nada pendente

            int needed = b >= 0xF0 ? 4 : b >= 0xE0 ? 3 : 2;
            return needed > i ? data.Length - i : data.Length;
        }

        return data.Length;
    }
}
