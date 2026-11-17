using Notas.Theme;

namespace Notas.UI.Commands;

/// <summary>Ctrl+Shift+P: todos os comandos com busca, no topo do editor (o protótipo esconde a barra de menus).</summary>
internal sealed partial class CommandPalette : Control
{
    private const int PaletteWidth = 560;
    private const int InputHeight = 50;
    private const int RowHeight = 32;
    private const int MaxRows = 10;

    private readonly TextBox _query = new()
    {
        BorderStyle = BorderStyle.None,
        BackColor = Palette.Background,
        ForeColor = Palette.Text,
        PlaceholderText = "Digite o nome de um comando",
    };

    private IReadOnlyList<PaletteItem> _items = [];
    private List<PaletteItem> _shown = [];
    private int _selected, _top;

    public CommandPalette()
    {
        SuspendLayout();
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.Selectable, false); // clicar numa linha não tira o foco da caixa de texto
        Visible = false;
        Controls.Add(_query);
        _query.TextChanged += (_, _) => Refilter();
        _query.KeyDown += OnQueryKeyDown;
        _query.LostFocus += (_, _) => Dismiss();
        ResumeLayout(false);
    }

    /// <summary>Item escolhido (Enter ou clique): quem abriu fecha a paleta e roda o comando.</summary>
    public event Action<PaletteItem>? Chosen;

    public void Open(IReadOnlyList<PaletteItem> items)
    {
        _items = items;
        _query.Text = "";
        Refilter();
        Visible = true;
        BringToFront();
        _query.Focus();
    }

    public void Dismiss() => Visible = false;
}
