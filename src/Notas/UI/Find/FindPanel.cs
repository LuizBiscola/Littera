using Notas.Editor.Search;
using Notas.Theme;
using ScintillaNET;

namespace Notas.UI.Find;

/// <summary>Painel de buscar/substituir do protótipo, flutuando no canto superior direito do editor.</summary>
internal sealed partial class FindPanel : Control
{
    private readonly Func<Scintilla?> _activeEditor;
    private readonly FindSession _session = new();
    private readonly TextBox _find = CreateBox();
    private readonly List<FindButton> _buttons = [];
    private readonly FindButton _matchCase, _wholeWord, _regex, _previous, _next, _close;

    public FindPanel(Func<Scintilla?> activeEditor)
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        _activeEditor = activeEditor;
        Visible = false;
        BackColor = Palette.Background;
        Controls.Add(_find);

        _matchCase = Add(FindButton.Label("Aa", "Diferenciar maiúsculas", () => Toggle(_matchCase!)));
        _wholeWord = Add(FindButton.Label("ab", "Palavra inteira", () => Toggle(_wholeWord!)));
        _regex = Add(FindButton.Label(".*", "Expressão regular", () => Toggle(_regex!)));
        _previous = Add(FindButton.Icon(Icons.ArrowUp, "Anterior (Shift+Enter)", () => Navigate(-1)));
        _next = Add(FindButton.Icon(Icons.ArrowDown, "Próxima (Enter)", () => Navigate(+1)));
        _close = Add(FindButton.Icon(Icons.Close, "Fechar (Esc)", () => Close()));
        AddReplace();
        AddScopes();
        _find.TextChanged += (_, _) => Search(select: true);
        HandleKeys(_find);
    }

    partial void Search(bool select);

    partial void Navigate(int step);

    partial void Close();

    partial void AddReplace();

    partial void AddScopes();

    private FindButton Add(FindButton button)
    {
        _buttons.Add(button);
        return button;
    }

    private void Toggle(FindButton option)
    {
        option.Toggled = !option.Toggled;
        Invalidate();
        Search(select: true);
    }

    private static TextBox CreateBox() =>
        new() { BorderStyle = System.Windows.Forms.BorderStyle.None, BackColor = Palette.Background, ForeColor = Palette.Text };
}
