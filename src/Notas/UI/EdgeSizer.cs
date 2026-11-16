using Notas.Theme;

namespace Notas.UI;

/// <summary>
/// Alça na borda entre um painel lateral e o editor: com o mouse em cima o cursor vira ↔ e a linha fica azul;
/// arrastar muda a largura e o clique duplo volta à padrão. Larguras em px de 96 dpi, como no protótipo.
/// </summary>
internal sealed partial class EdgeSizer(Control panel, bool rightEdge, int defaultWidth)
{
    public bool Hot { get; private set; }

    private Rectangle Line => new(rightEdge ? panel.Width - panel.Px(2) : 0, 0, panel.Px(2), panel.Height);

    private bool Contains(Point p) => rightEdge ? p.X >= panel.Width - panel.Px(6) : p.X < panel.Px(6);

    /// <summary>Durante o arrasto (<see cref="Dragging"/>) o painel ignora o mouse; fora dele, segue com o hover normal.</summary>
    public void MouseMove(MouseEventArgs e)
    {
        if (Dragging)
            DragTo(panel.PointToScreen(e.Location).X);
        else
            SetHot(e.Button == MouseButtons.None && Contains(e.Location));
    }

    public void MouseLeave()
    {
        if (!Dragging)
            SetHot(false);
    }

    public void Paint(Graphics g)
    {
        if (!Hot && !Dragging)
            return;
        using var accent = new SolidBrush(Palette.Accent);
        g.FillRectangle(accent, Line);
    }

    private void SetHot(bool hot)
    {
        if (hot == Hot)
            return;
        Hot = hot;
        panel.Cursor = hot ? Cursors.SizeWE : Cursors.Default;
        panel.Invalidate(Line);
    }
}
