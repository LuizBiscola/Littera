using ScintillaNET;

namespace Littera.Editor.Search;

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
        if (query.Text.Length == 0 || range.End <= range.Start)
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

    /// <summary>Substitui tudo num único "desfazer". Com regex, a substituição aceita os grupos ($1).</summary>
    public static int ReplaceAll(Scintilla editor, SearchQuery query, string replacement, TextRange range)
    {
        if (query.Text.Length == 0)
            return 0;

        int count = 0, end = range.End;
        editor.SearchFlags = query.Flags;
        editor.BeginUndoAction();
        try
        {
            editor.SetTargetRange(range.Start, end);
            while (editor.SearchInTarget(query.Text) != -1)
            {
                int matched = editor.TargetEnd - editor.TargetStart;
                int written = query.Regex ? editor.ReplaceTargetRe(replacement) : editor.ReplaceTarget(replacement);
                count++;
                end += written - matched;
                int next = editor.TargetEnd + (matched == 0 ? 1 : 0);
                if (next > end)
                    break;
                editor.SetTargetRange(next, end);
            }
        }
        finally
        {
            editor.EndUndoAction();
        }

        return count;
    }
}
