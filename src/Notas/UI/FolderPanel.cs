using Notas.Editor;
using Notas.Theme;

namespace Notas.UI;

/// <summary>Painel "PASTA" do protótipo: árvore da pasta aberta, com filtro, à direita do editor.</summary>
internal sealed partial class FolderPanel : Control
{
    private const int PanelWidth = 264;
    private const int HeaderHeight = 134;
    private const int FooterHeight = 35;

    private readonly DocumentManager _documents;
    private readonly HashSet<string> _expanded = new(StringComparer.OrdinalIgnoreCase);

    public FolderPanel(DocumentManager documents)
    {
        SuspendLayout();
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        _documents = documents;
        _documents.Changed += Invalidate; // marcas de "aberto em aba" e "não salvo"
        Dock = DockStyle.Right;
        Width = this.Px(PanelWidth);
        CreateFilter();
        ResumeLayout(false);
    }

    public string? Folder { get; private set; }

    public IEnumerable<string> ExpandedFolders => _expanded;

    /// <summary>Clique num arquivo da árvore.</summary>
    public event Action<string>? FileRequested;

    /// <summary>Botão "Fechar pasta".</summary>
    public event Action? CloseRequested;

    protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
    {
        base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        Width = this.Px(PanelWidth);
    }
}
