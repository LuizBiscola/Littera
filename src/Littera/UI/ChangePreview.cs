using Littera.Editor.Changes;
using Littera.Theme;

namespace Littera.UI;

/// <summary>Prévia da alteração (protótipo "Alterações nas linhas"): linhas removidas e novas em relação ao arquivo salvo.</summary>
internal sealed partial class ChangePreview : Control
{
    private const int PreviewWidth = 520;
    private const int HeaderHeight = 38;
    private const int FooterHeight = 30;
    private const int RowHeight = 20;
    private const int MaxRows = 14;

    private DiffHunk _hunk;
    private string[] _removed = [], _added = [];

    public ChangePreview()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        Visible = false;
    }

    /// <summary>Botão "Reverter": quem abriu devolve as linhas da versão salva.</summary>
    public event Action<DiffHunk, string[]>? RevertRequested;

    /// <summary>Atalho mostrado no rodapé (o configurado para "proximaAlteracao").</summary>
    public string? Shortcut { get; set; }

    /// <summary>Linha onde a prévia foi aberta; -1 fechada.</summary>
    public int AnchorLine { get; private set; } = -1;

    public void Show(DiffHunk hunk, string[] saved, string[] current, int line, Point anchor)
    {
        _hunk = hunk;
        _removed = saved[hunk.BeforeStart..(hunk.BeforeStart + hunk.BeforeCount)];
        _added = current[hunk.AfterStart..(hunk.AfterStart + hunk.AfterCount)];
        AnchorLine = line;
        int rows = Math.Min(_removed.Length + _added.Length, MaxRows) + (_removed.Length + _added.Length > MaxRows ? 1 : 0);
        Size = new Size(this.Px(PreviewWidth), this.Px(HeaderHeight + FooterHeight + 12) + rows * this.Px(RowHeight));
        Location = anchor;
        Visible = true;
        BringToFront();
        Invalidate();
    }

    public void Dismiss()
    {
        Visible = false;
        AnchorLine = -1;
    }
}
