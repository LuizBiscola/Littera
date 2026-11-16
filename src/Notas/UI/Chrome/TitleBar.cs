using Notas.Theme;

namespace Notas.UI.Chrome;

/// <summary>Barra de título do protótipo: marca, botões do painel, título centralizado, ações e botões da janela.</summary>
internal sealed partial class TitleBar : Control
{
    private readonly List<ChromeButton> _left = [];
    private readonly List<ChromeButton> _actions = [];
    private readonly ChromeButton _minimize;
    private readonly ChromeButton _maximize;
    private readonly ChromeButton _close;

    public TitleBar()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        Dock = DockStyle.Top;
        Height = this.Px(40);
        _minimize = new(Icons.Minimize, "Minimizar", () => FindForm()!.WindowState = FormWindowState.Minimized) { IsWindowButton = true };
        _maximize = new(Icons.Maximize, "Maximizar", ToggleMaximize) { IsWindowButton = true };
        _close = new(Icons.Close, "Fechar", () => FindForm()!.Close()) { IsWindowButton = true, IsClose = true };
    }

    /// <summary>Área do maximizar: o formulário responde HTMAXBUTTON aqui (Snap Layouts do Windows 11).</summary>
    public Rectangle MaximizeBounds => _maximize.Bounds;

    public ChromeButton AddLeft(char glyph, string toolTip, Action onClick) => Add(_left, glyph, toolTip, onClick);

    public ChromeButton AddAction(char glyph, string toolTip, Action onClick) => Add(_actions, glyph, toolTip, onClick);

    public void ToggleMaximize()
    {
        Form form = FindForm()!;
        form.WindowState = form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
    }

    private ChromeButton Add(List<ChromeButton> list, char glyph, string toolTip, Action onClick)
    {
        var button = new ChromeButton(glyph, toolTip, onClick);
        list.Add(button);
        PerformLayout();
        return button;
    }

    private IEnumerable<ChromeButton> AllButtons => [.. _left, .. _actions, _minimize, _maximize, _close];
}
