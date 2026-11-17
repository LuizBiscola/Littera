using Notas.Services;

namespace Notas.Editor.Search;

internal static partial class FileSearch
{
    private const int MaxFileBytes = 8 * 1024 * 1024;

    /// <summary>Texto no encoding detectado; null para binários, arquivos grandes demais ou ilegíveis.</summary>
    public static string? ReadText(string path)
    {
        try
        {
            if (new FileInfo(path).Length > MaxFileBytes)
                return null;
            byte[] bytes = File.ReadAllBytes(path);
            using var stream = new MemoryStream(bytes);
            TextEncodingKind kind = EncodingDetector.Detect(stream);
            bool utf16 = kind is TextEncodingKind.Utf16LE or TextEncodingKind.Utf16BE;
            if (!utf16 && bytes.AsSpan(0, Math.Min(bytes.Length, 8000)).Contains((byte)0))
                return null; // NUL nos primeiros bytes: binário
            using var reader = new StreamReader(stream, kind.ToEncoding(), detectEncodingFromByteOrderMarks: true);
            return reader.ReadToEnd();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }
}
