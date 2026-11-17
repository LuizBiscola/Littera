using System.Text;

namespace Littera.Services;

internal static class TextFormatExtensions
{
    private static readonly Lazy<Encoding> Windows1252 = new(() =>
    {
        // O provedor de code pages só é registrado quando aparece um arquivo ANSI.
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        return Encoding.GetEncoding(1252);
    });

    public static string Label(this TextEncodingKind kind) => kind switch
    {
        TextEncodingKind.Utf8 => "UTF-8",
        TextEncodingKind.Utf8Bom => "UTF-8 BOM",
        TextEncodingKind.Ansi => "ANSI",
        TextEncodingKind.Utf16LE => "UTF-16 LE",
        _ => "UTF-16 BE",
    };

    public static string Label(this LineEnding ending) => ending switch
    {
        LineEnding.CrLf => "CRLF",
        LineEnding.Lf => "LF",
        _ => "CR",
    };

    /// <summary>ANSI = Windows-1252. Os encodings com BOM gravam o BOM ao salvar.</summary>
    public static Encoding ToEncoding(this TextEncodingKind kind) => kind switch
    {
        TextEncodingKind.Utf8 => new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
        TextEncodingKind.Utf8Bom => new UTF8Encoding(encoderShouldEmitUTF8Identifier: true),
        TextEncodingKind.Utf16LE => new UnicodeEncoding(bigEndian: false, byteOrderMark: true),
        TextEncodingKind.Utf16BE => new UnicodeEncoding(bigEndian: true, byteOrderMark: true),
        _ => Windows1252.Value,
    };
}
