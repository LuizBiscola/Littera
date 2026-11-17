using Littera.Editor;
using Littera.Theme;

namespace Littera.UI;

/// <summary>
/// Painel de abas do protótipo: seção Fixadas (só quando há alguma) e Abertos, com alfinete, ×,
/// bolinha de "não salvo", arrastar para reordenar e menu do clique direito.
/// </summary>
internal sealed partial class OpenFilesPanel : Control
{
    private const int PanelWidth = 250;
    private const int PinnedHeaderHeight = 36;
    private const int SeparatorHeight = 11;
    private const int OpenHeaderHeight = 42;
    private const int SoloHeaderHeight = 48; // sem fixadas, o cabeçalho do protótipo original
    private const int FooterHeight = 35;
    private const int ItemHeight = 32;
    private const int ItemStep = 34;

    private readonly DocumentManager _documents;
    private readonly EdgeSizer _sizer;
    private int _width = PanelWidth;

    public OpenFilesPanel(DocumentManager documents)
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        _documents = documents;
        _documents.Changed += Invalidate;
        Dock = DockStyle.Left;
        Width = this.Px(PanelWidth);
        _sizer = new EdgeSizer(this, rightEdge: true, PanelWidth);
        _sizer.Resized += width =>
        {
            _width = width;
            WidthChosen?.Invoke(width);
        };
        _tipTimer.Tick += (_, _) => ShowTip();
        _flashTimer.Tick += (_, _) => OnFlashTick();
    }

    /// <summary>Fechar pela aba (× ou botão do meio): quem trata pergunta se quer salvar.</summary>
    public event Action<OpenDocument>? CloseRequested;

    public event Action? NewRequested;

    /// <summary>Clique direito numa aba: o menu fica com a janela, que tem as ações (renomear, mostrar na pasta…).</summary>
    public event Action<OpenDocument, Point>? MenuRequested;

    /// <summary>Atalho configurado de um comando, para as dicas ("Fixar aba · Ctrl+Alt+P").</summary>
    public Func<string, string?>? ShortcutText { get; set; }

    /// <summary>Largura arrastada na borda (px de 96 dpi), para guardar nas configurações.</summary>
    public event Action<int>? WidthChosen;

    /// <summary>Largura guardada (px de 96 dpi); valores fora da faixa da alça são corrigidos.</summary>
    public int ChosenWidth
    {
        get => _width;
        set => Width = this.Px(_width = Math.Clamp(value, EdgeSizer.MinWidth, EdgeSizer.MaxWidth));
    }

    /// <summary>Fixa ou desafixa, com a confirmação azul ao fixar.</summary>
    public void TogglePin(OpenDocument document)
    {
        bool pin = !document.Pinned;
        _documents.SetPinned(document, pin);
        if (pin)
            Flash(document);
    }

    protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
    {
        base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        Width = this.Px(_width);
    }
}
