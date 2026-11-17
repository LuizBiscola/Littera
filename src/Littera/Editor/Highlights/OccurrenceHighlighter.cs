using Littera.Theme;
using ScintillaNET;

namespace Littera.Editor.Highlights;

/// <summary>"Destacar ocorrências da palavra": fundo amarelo em cada ocorrência, mais forte na do cursor.</summary>
internal static class OccurrenceHighlighter
{
    private const int Other = 8;
    private const int Current = 9;
    private const int MaxMatches = 5000;

    public static void Setup(Scintilla editor)
    {
        foreach ((int id, Color color) in new[] { (Other, Palette.Match), (Current, Palette.MatchCurrent) })
        {
            Indicator indicator = editor.Indicators[id];
            indicator.Style = IndicatorStyle.RoundBox;
            indicator.ForeColor = color;
            indicator.Alpha = 255;
            indicator.OutlineAlpha = 255;
            indicator.Under = true; // atrás do texto, que continua com a cor da sintaxe
        }
    }

    /// <summary>Destaca a palavra sob o cursor (inteira, sem diferenciar maiúsculas); devolve "palavra · 7 ocorrências".</summary>
    public static string? Update(Scintilla editor)
    {
        Clear(editor);
        int caret = editor.CurrentPosition;
        int start = editor.WordStartPosition(caret, true);
        int end = editor.WordEndPosition(caret, true);
        if (end - start < 2 || editor.Selections.Count > 1)
            return null;

        string word = editor.GetTextRange(start, end - start);
        editor.SearchFlags = SearchFlags.WholeWord;
        editor.TargetWholeDocument();
        int count = 0;
        while (count < MaxMatches && editor.SearchInTarget(word) != -1)
        {
            int found = editor.TargetStart, length = editor.TargetEnd - found;
            editor.IndicatorCurrent = found == start ? Current : Other;
            editor.IndicatorFillRange(found, length);
            count++;
            editor.SetTargetRange(found + length, editor.TextLength);
        }

        if (count >= 2)
            return $"{word} · {count} ocorrências";
        Clear(editor); // só ela mesma: não vale destacar
        return null;
    }

    public static void Clear(Scintilla editor)
    {
        foreach (int id in new[] { Other, Current })
        {
            editor.IndicatorCurrent = id;
            editor.IndicatorClearRange(0, editor.TextLength);
        }
    }
}
