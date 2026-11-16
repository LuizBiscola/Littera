using Notas.Editor.Search;
using ScintillaNET;

namespace Notas.UI.Find;

internal sealed partial class FindPanel
{
    private SearchQuery Query => new(_find.Text, _matchCase.Toggled, _wholeWord.Toggled, _regex.Toggled);

    /// <summary>Refaz a busca; com <paramref name="select"/>, leva o editor até a ocorrência a partir do cursor.</summary>
    partial void Search(bool select)
    {
        if (_activeEditor() is not { } editor)
            return;

        _session.Run(editor, Query, ScopeRange(editor), editor.SelectionStart);
        if (select)
            _session.Select(editor);
        Invalidate();
    }

    partial void Navigate(int step)
    {
        if (_activeEditor() is not { } editor)
            return;
        Search(select: false); // o texto pode ter mudado desde a última busca
        _session.Select(editor, editor.SelectionStart == editor.SelectionEnd ? 0 : step);
        Invalidate();
    }

    private TextRange ScopeRange(Scintilla editor) => _scope switch
    {
        SearchScope.Selection when _selectionScope is { } selection => selection,
        SearchScope.Block when BlockRange?.Invoke() is { } block => block,
        _ => new TextRange(0, editor.TextLength),
    };
}
