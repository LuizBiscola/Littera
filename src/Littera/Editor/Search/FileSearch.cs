using System.Text.RegularExpressions;

namespace Littera.Editor.Search;

/// <summary>Busca fora do editor (abas ainda não carregadas e arquivos da pasta) com as opções do painel.</summary>
internal static partial class FileSearch
{
    /// <summary>null quando o texto está vazio ou a expressão regular é inválida.</summary>
    public static Regex? ToRegex(SearchQuery query)
    {
        if (query.Text.Length == 0)
            return null;

        string pattern = query.Regex ? query.Text : Regex.Escape(query.Text);
        if (query.WholeWord)
            pattern = $@"(?<!\w)(?:{pattern})(?!\w)";
        RegexOptions options = RegexOptions.Multiline | RegexOptions.CultureInvariant
            | (query.MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase);
        try
        {
            return new Regex(pattern, options, TimeSpan.FromSeconds(2));
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    public static int Count(string text, Regex regex)
    {
        int count = 0;
        try
        {
            for (Match match = regex.Match(text); match.Success && count < TextSearch.MaxMatches; match = match.NextMatch())
                count++;
        }
        catch (RegexMatchTimeoutException)
        {
            // Expressão patológica: conta o que achou até aqui.
        }

        return count;
    }
}
