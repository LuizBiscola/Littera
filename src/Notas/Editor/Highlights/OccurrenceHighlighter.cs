using Notas.Theme;
using ScintillaNET;

namespace Notas.Editor.Highlights;

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

    public static void Clear(Scintilla editor)
    {
        foreach (int id in new[] { Other, Current })
        {
            editor.IndicatorCurrent = id;
            editor.IndicatorClearRange(0, editor.TextLength);
        }
    }
}
