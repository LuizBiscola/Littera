namespace Littera.Editor.Folding;

internal static partial class FoldScanner
{
    /// <summary>Fim de um identificador; "@begin" e "#temp" contam inteiros, para não virarem BEGIN ou TEMP.</summary>
    private static int WordEnd(string text, int i)
    {
        while (i < text.Length && (char.IsLetterOrDigit(text[i]) || text[i] is '_' or '@' or '#' or '$'))
            i++;
        return i;
    }

    private static bool IsWord(ReadOnlySpan<char> word, string keyword) => word.Equals(keyword, StringComparison.OrdinalIgnoreCase);

    /// <summary>BEGIN TRAN, TRANSACTION ou DISTRIBUTED não abre bloco: termina em COMMIT ou ROLLBACK, não em END.</summary>
    private static bool IsTransaction(string text, int i)
    {
        while (i < text.Length && char.IsWhiteSpace(text[i]))
            i++;
        ReadOnlySpan<char> next = text.AsSpan(i, WordEnd(text, i) - i);
        return IsWord(next, "TRAN") || IsWord(next, "TRANSACTION") || IsWord(next, "DISTRIBUTED");
    }
}
