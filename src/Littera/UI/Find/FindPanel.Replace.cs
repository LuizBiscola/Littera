using Notas.Editor.Search;
using Notas.Theme;

namespace Notas.UI.Find;

internal sealed partial class FindPanel
{
    private TextBox? _replace;
    private FindButton? _expand, _replaceOne, _replaceAll;
    private bool _replaceShown; // Visible de um filho é falso enquanto o painel está escondido

    partial void AddReplace()
    {
        _replace = CreateBox();
        _replace.Visible = false;
        Controls.Add(_replace);
        HandleKeys(_replace);
        _expand = Add(FindButton.Icon(Icons.ChevronRight, "Mostrar substituir (Ctrl+H)", () => ShowReplace(!_replaceShown)));
        _replaceOne = Add(FindButton.Label("Substituir", "Substituir esta ocorrência e ir para a próxima", ReplaceCurrent));
        _replaceAll = Add(FindButton.Label("Todas", "Substituir todas no escopo", ReplaceAll));
    }

    partial void ShowReplace(bool replace)
    {
        _replaceShown = replace;
        _replace!.Visible = _replaceOne!.Visible = _replaceAll!.Visible = replace;
        _expand!.Glyph = replace ? Icons.ChevronDown : Icons.ChevronRight;
        PerformLayout();
        Invalidate();
    }

    private void ReplaceCurrent()
    {
        if (_activeEditor() is not { } editor)
            return;
        Navigate(0); // garante que a seleção é uma ocorrência
        if (_session.Count == 0)
            return;
        editor.ReplaceSelection(_replace!.Text);
        Navigate(+1);
    }

    private void ReplaceAll()
    {
        if (MultiFile)
        {
            _ = ReplaceInFilesAsync();
            return;
        }

        if (_activeEditor() is not { } editor)
            return;
        int count = TextSearch.ReplaceAll(editor, Query, _replace!.Text, ScopeRange(editor));
        Search(select: false);
        _toolTip ??= new ToolTip();
        _toolTip.Show($"{count} substituições", this, _replaceAll!.Bounds.X, Height, 1500);
    }
}
