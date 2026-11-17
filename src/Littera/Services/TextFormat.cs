namespace Littera.Services;

internal enum TextEncodingKind
{
    Utf8,
    Utf8Bom,
    Ansi,
    Utf16LE,
    Utf16BE,
}

internal enum LineEnding
{
    CrLf,
    Lf,
    Cr,
}

/// <summary>Encoding e fim de linha de um arquivo: o que é detectado ao abrir e usado ao salvar.</summary>
internal sealed record TextFormat(TextEncodingKind Encoding, LineEnding LineEnding)
{
    public static TextFormat Default { get; } = new(TextEncodingKind.Utf8, LineEnding.CrLf);
}
