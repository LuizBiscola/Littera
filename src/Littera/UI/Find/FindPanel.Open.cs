using Littera.Editor.Search;

namespace Littera.UI.Find;

internal sealed partial class FindPanel
{
    /// <summary>Abre com o texto selecionado (se for de uma linha só) e foca a caixa de busca.</summary>
    public void Open(bool replace)
    {
        if (_activeEditor() is { } editor)
        {
            string selected = editor.SelectedText;
            bool multiLine = selected.Contains('\n');
            CaptureSelection(multiLine ? new TextRange(editor.SelectionStart, editor.SelectionEnd) : null);
            if (selected.Length > 0 && !multiLine)
                _find.Text = selected;
        }

        ShowReplace(replace);
        Visible = true;
        BringToFront();
        _find.Focus();
        _find.SelectAll();
        Search(select: false);
    }

    /// <summary>F3 / Shift+F3: com o painel fechado, abre; aberto, vai para a próxima/anterior.</summary>
    public void Step(int step)
    {
        if (Visible && _find.Text.Length > 0)
            Navigate(step);
        else
            Open(replace: false);
    }

    public void Dismiss() => Close();

    partial void CaptureSelection(TextRange? selection);

    partial void ShowReplace(bool replace);

    partial void Close()
    {
        Visible = false;
        if (_activeEditor() is { } editor)
        {
            FindSession.Clear(editor);
            editor.Focus();
        }
    }
}
