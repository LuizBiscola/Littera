using ScintillaNET;

namespace Notas.Editor.Search;

internal sealed record SearchQuery(string Text, bool MatchCase, bool WholeWord, bool Regex)
{
    /// <summary>Regex no padrão ECMAScript do std::regex (o regex nativo do Scintilla é bem limitado).</summary>
    public SearchFlags Flags =>
        (MatchCase ? SearchFlags.MatchCase : 0) | (WholeWord ? SearchFlags.WholeWord : 0)
        | (Regex ? SearchFlags.Regex | SearchFlags.Cxx11Regex : 0);
}

internal readonly record struct TextRange(int Start, int End);

/// <summary>Busca e substituição no editor usando o "target" do Scintilla.</summary>
internal static class TextSearch
{
    public const int MaxMatches = 10_000;

    public static List<TextRange> FindAll(Scintilla editor, SearchQuery query, TextRange range)
    {
        var matches = new List<TextRange>();
        if (query.Text.Length == 0)
            return matches;

        editor.SearchFlags = query.Flags;
        editor.SetTargetRange(range.Start, range.End);
        while (matches.Count < MaxMatches && editor.SearchInTarget(query.Text) != -1)
        {
            int start = editor.TargetStart, end = editor.TargetEnd;
            matches.Add(new TextRange(start, end));
            editor.SetTargetRange(end > start ? end : end + 1, range.End); // regex vazio (ex.: ^) não pode travar o laço
            if (end >= range.End)
                break;
        }

        return matches;
    }
}
