namespace Littera.UI;

internal sealed partial class ChangePreview
{
    /// <summary>"Linhas 17–19"; remoção pura mostra onde as linhas estavam.</summary>
    private string Title => _hunk.AfterCount switch
    {
        0 => $"Linha {_hunk.AfterStart + 1}",
        1 => $"Linha {_hunk.AfterStart + 1}",
        _ => $"Linhas {_hunk.AfterStart + 1}–{_hunk.AfterStart + _hunk.AfterCount}",
    };

    private string Subtitle => WhitespaceOnly ? "não salvas · só espaços em branco"
        : $"não salvas · {Count(_removed.Length, "removida", "removidas")}, {Count(_added.Length, "nova", "novas")}";

    /// <summary>Removidas e novas iguais tirando espaços e tabs (inclusive linha em branco a mais ou a menos).</summary>
    private bool WhitespaceOnly => Squeeze(_removed) == Squeeze(_added);

    /// <summary>
    /// Espaço no fim da linha vira "·" (senão as duas versões pareceriam iguais); quando a diferença é só
    /// de espaços, todos aparecem, e tab vira "→".
    /// </summary>
    private string ShowSpaces(string text)
    {
        if (WhitespaceOnly)
            return text.Replace(' ', '·').Replace("\t", "→   ");
        int end = text.TrimEnd().Length;
        return text[..end].Replace("\t", "    ") + text[end..].Replace(' ', '·').Replace("\t", "→   ");
    }

    private static string Squeeze(string[] lines) => string.Concat(lines.SelectMany(l => l.Where(c => !char.IsWhiteSpace(c))));

    private static string Count(int n, string one, string many) => $"{n} {(n == 1 ? one : many)}";
}
