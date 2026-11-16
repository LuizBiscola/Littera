using Notas.Editor.Search;

namespace Notas.UI.Find;

internal sealed partial class FindPanel
{
    private FindButton[] _scopes = [];
    private SearchScope _scope = SearchScope.File;
    private TextRange? _selectionScope;

    /// <summary>Faixa do bloco atual (instrução SQL ou parágrafo), fornecida pela janela.</summary>
    public Func<TextRange?>? BlockRange { get; set; }

    partial void AddScopes()
    {
        _scopes =
        [
            Add(FindButton.Label("Arquivo", "Buscar no arquivo inteiro", () => SetScope(SearchScope.File))),
            Add(FindButton.Label("Bloco atual", "Só na instrução ou parágrafo do cursor", () => SetScope(SearchScope.Block))),
            Add(FindButton.Label("Seleção", "Só no trecho selecionado ao abrir a busca", () => SetScope(SearchScope.Selection))),
        ];
        _scopes[0].Toggled = true;
    }

    /// <summary>Seleção de várias linhas ao abrir vira o escopo automaticamente.</summary>
    partial void CaptureSelection(TextRange? selection)
    {
        _selectionScope = selection;
        SetScope(selection is null ? SearchScope.File : SearchScope.Selection);
    }

    private void SetScope(SearchScope scope)
    {
        _scope = scope;
        for (int i = 0; i < _scopes.Length; i++)
            _scopes[i].Toggled = (int)scope == i;
        Search(select: false);
        Invalidate();
    }
}

internal enum SearchScope
{
    File,
    Block,
    Selection,
}
