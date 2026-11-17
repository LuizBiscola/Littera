using Littera.Editor;
using Littera.Theme;

namespace Littera.UI;

/// <summary>Painel "PASTA" do protótipo: árvore da pasta aberta, com filtro, à direita do editor.</summary>
internal sealed partial class FolderPanel : Control
{
    private const int PanelWidth = 264;
    private const int HeaderHeight = 134;
    private const int FooterHeight = 35;

    private readonly DocumentManager _documents;
    private readonly HashSet<string> _expanded = new(StringComparer.OrdinalIgnoreCase);
    private readonly EdgeSizer _sizer;
    private int _width = PanelWidth;

    public FolderPanel(DocumentManager documents)
    {
        SuspendLayout();
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        _documents = documents;
        _documents.Changed += Invalidate; // marcas de "aberto em aba" e "não salvo"
        Dock = DockStyle.Right;
        Width = this.Px(PanelWidth);
        _sizer = new EdgeSizer(this, rightEdge: false, PanelWidth);
        _sizer.Resized += width =>
        {
            _width = width;
            WidthChosen?.Invoke(width);
        };
        CreateFilter();
        _watchTimer.Tick += (_, _) =>
        {
            _watchTimer.Stop();
            Reload();
        };
        ResumeLayout(false);
    }

    public string? Folder { get; private set; }

    public IEnumerable<string> ExpandedFolders => _expanded;

    /// <summary>Clique num arquivo da árvore.</summary>
    public event Action<string>? FileRequested;

    /// <summary>Botão "Fechar pasta".</summary>
    public event Action? CloseRequested;

    /// <summary>Largura arrastada na borda (px de 96 dpi), para guardar nas configurações.</summary>
    public event Action<int>? WidthChosen;

    /// <summary>Largura guardada (px de 96 dpi); valores fora da faixa da alça são corrigidos.</summary>
    public int ChosenWidth
    {
        get => _width;
        set => Width = this.Px(_width = Math.Clamp(value, EdgeSizer.MinWidth, EdgeSizer.MaxWidth));
    }

    protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
    {
        base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        Width = this.Px(_width);
    }
}
