namespace Littera.UI.Commands;

/// <summary>Uma linha da paleta: nome, atalho (desenhado como tecla), detalhe em cinza (a pasta de um recente) e a ação.</summary>
internal sealed record PaletteItem(string Label, string? Shortcut, string? Detail, Action Run);

/// <summary>Filtro da paleta: cada palavra digitada precisa aparecer no nome, sem ligar para maiúsculas e acentos.</summary>
internal static class CommandFilter
{
    public static List<PaletteItem> Filter(IReadOnlyList<PaletteItem> items, string query)
    {
        string[] words = Fold(query).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0)
            return [.. items];

        return [.. items
            .Select((item, index) => (Item: item, Index: index, Text: Fold(item.Label)))
            .Where(x => words.All(w => x.Text.Contains(w, StringComparison.Ordinal)))
            .OrderBy(x => Rank(x.Text, words))
            .ThenBy(x => x.Index)
            .Select(x => x.Item)];
    }

    /// <summary>0: o nome começa com a primeira palavra; 1: cada palavra começa uma palavra do nome; 2: aparecem no meio.</summary>
    private static int Rank(string text, string[] words)
    {
        if (text.StartsWith(words[0], StringComparison.Ordinal))
            return 0;
        return words.All(w => text.StartsWith(w, StringComparison.Ordinal) || text.Contains(" " + w, StringComparison.Ordinal)) ? 1 : 2;
    }

    /// <summary>Minúsculas sem acento ("Configurações" → "configuracoes"), sem depender da cultura do Windows.</summary>
    public static string Fold(string text)
    {
        char[] chars = text.ToLowerInvariant().ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = chars[i] switch
            {
                'á' or 'à' or 'â' or 'ã' or 'ä' => 'a',
                'é' or 'è' or 'ê' or 'ë' => 'e',
                'í' or 'ì' or 'î' or 'ï' => 'i',
                'ó' or 'ò' or 'ô' or 'õ' or 'ö' => 'o',
                'ú' or 'ù' or 'û' or 'ü' => 'u',
                'ç' => 'c',
                char c => c,
            };
        }

        return new string(chars);
    }
}
