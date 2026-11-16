using Notas.Theme;

namespace Notas.UI;

internal sealed partial class EdgeSizer
{
    public const int MinWidth = 180;
    public const int MaxWidth = 600;
    private const int MinEditor = 320; // o editor nunca fica mais estreito que isso

    private int _startX;
    private int _startWidth;

    public bool Dragging { get; private set; }

    /// <summary>Largura escolhida (px de 96 dpi), ao soltar ou no clique duplo.</summary>
    public event Action<int>? Resized;

    public bool MouseDown(MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left || !Contains(e.Location))
            return false;
        if (e.Clicks == 2)
        {
            Apply(panel.Px(defaultWidth));
            Resized?.Invoke(defaultWidth);
            return true;
        }

        (Dragging, _startX, _startWidth) = (true, panel.PointToScreen(e.Location).X, panel.Width);
        panel.Capture = true;
        return true;
    }

    public bool MouseUp(MouseEventArgs e)
    {
        if (!Dragging)
            return false;
        Dragging = false;
        panel.Capture = false;
        Resized?.Invoke((int)Math.Round(panel.Width * 96.0 / panel.DeviceDpi));
        SetHot(Contains(e.Location));
        return true;
    }

    /// <summary>Posição na tela, não no painel: o painel da direita anda junto com a borda.</summary>
    private void DragTo(int screenX)
    {
        int delta = screenX - _startX;
        Apply(_startWidth + (rightEdge ? delta : -delta));
    }

    /// <summary>Entre o mínimo e o máximo, sem espremer o editor (o outro painel lateral também conta).</summary>
    private void Apply(int width)
    {
        int others = panel.Parent?.Controls.Cast<Control>()
            .Where(c => c != panel && c.Visible && c.Dock is DockStyle.Left or DockStyle.Right).Sum(c => c.Width) ?? 0;
        int room = (panel.Parent?.ClientSize.Width ?? int.MaxValue) - others - panel.Px(MinEditor);
        int max = Math.Max(panel.Px(MinWidth), Math.Min(panel.Px(MaxWidth), room));
        panel.Width = Math.Clamp(width, panel.Px(MinWidth), max);
    }
}
