namespace Littera.Editor.Folding;

/// <summary>Resumo mostrado no lugar do trecho recolhido: "8 linhas │ SELECT 4 colunas · FROM pedidos · WHERE 2 filtros".</summary>
internal static partial class FoldSummary
{
    private const int MaxDetail = 60;

    public static string For(IReadOnlyList<string> hidden, bool sql)
    {
        string count = hidden.Count == 1 ? "1 linha" : $"{hidden.Count} linhas";
        string? detail = sql ? SqlDetail(string.Join("\n", hidden)) : null;
        return $"{count} │ {detail ?? FirstLine(hidden)}{Closing(hidden)}";
    }

    /// <summary>A última linha escondida só fecha o trecho (")", ") AS t", "END", "END TRY"): aparece no fim do resumo.</summary>
    private static string Closing(IReadOnlyList<string> hidden)
    {
        string last = hidden[^1].Trim();
        bool closes = last.StartsWith(')') || last.Equals("END", StringComparison.OrdinalIgnoreCase)
            || last.StartsWith("END ", StringComparison.OrdinalIgnoreCase) || last.StartsWith("END;", StringComparison.OrdinalIgnoreCase);
        return closes && last.Length <= 16 ? "  " + last : "";
    }

    private static string FirstLine(IReadOnlyList<string> hidden)
    {
        string first = hidden.FirstOrDefault(l => !string.IsNullOrWhiteSpace(l))?.Trim() ?? "";
        return Shorten(first, MaxDetail);
    }

    private static string Shorten(string text, int max) => text.Length <= max ? text : text[..(max - 1)].TrimEnd() + "…";
}
