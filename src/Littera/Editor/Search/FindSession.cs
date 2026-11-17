using Littera.Theme;
using ScintillaNET;

namespace Littera.Editor.Search;

/// <summary>Uma busca em andamento: ocorrências no escopo, qual está selecionada e o destaque de todas.</summary>
internal sealed class FindSession
{
    private const int Indicator = 10;
    private List<TextRange> _matches = [];
    private Scintilla? _editor;

    public int Count => _matches.Count;

    public int Current { get; private set; } = -1;

    public string Label => Count == 0 ? "Nenhum resultado"
        : $"{Current + 1} de {Count}{(Count >= TextSearch.MaxMatches ? "+" : "")}";

    public static void Setup(Scintilla editor)
    {
        Indicator indicator = editor.Indicators[Indicator];
        indicator.Style = IndicatorStyle.RoundBox;
        indicator.ForeColor = Palette.Match;
        indicator.Alpha = 255;
        indicator.OutlineAlpha = 255;
        indicator.Under = true;
    }

    /// <summary>Refaz a busca e escolhe a primeira ocorrência a partir de <paramref name="from"/>.</summary>
    public void Run(Scintilla editor, SearchQuery query, TextRange scope, int from)
    {
        if (_editor is { IsDisposed: false } previous && previous != editor)
            Clear(previous); // a busca passou para outra aba: o destaque não fica para trás
        _editor = editor;
        Clear(editor);
        _matches = TextSearch.FindAll(editor, query, scope);
        editor.IndicatorCurrent = Indicator;
        foreach (TextRange match in _matches)
            editor.IndicatorFillRange(match.Start, match.End - match.Start);

        Current = _matches.FindIndex(m => m.Start >= from);
        AfterLast = Current < 0;
        if (Current < 0 && Count > 0)
            Current = 0;
    }

    /// <summary>O cursor estava depois da última ocorrência (Current voltou para a primeira).</summary>
    public bool AfterLast { get; private set; }

    /// <summary>Seleciona a ocorrência atual (ou a seguinte/anterior) e rola até ela.</summary>
    public void Select(Scintilla editor, int step = 0)
    {
        if (Count == 0)
            return;
        Current = (Current + step + Count) % Count;
        TextRange match = _matches[Current];
        editor.Lines[editor.LineFromPosition(match.Start)].EnsureVisible(); // ocorrência em trecho recolhido: expande
        editor.SetSelection(match.End, match.Start);
        editor.ScrollRange(match.Start, match.End);
    }

    public static void Clear(Scintilla editor)
    {
        editor.IndicatorCurrent = Indicator;
        editor.IndicatorClearRange(0, editor.TextLength);
    }
}
