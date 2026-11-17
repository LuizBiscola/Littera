namespace Notas.UI;

internal sealed partial class StatusBar
{
    private StatusSegment? _hover;

    /// <summary>Clique num campo da direita; o retângulo (em tela) serve para posicionar o menu.</summary>
    public event Action<StatusSegment, Rectangle>? SegmentClicked;

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        StatusSegment? hit = SegmentAt(e.Location);
        Cursor = hit is null && !OverExpandAll(e.Location) && !OverChanges(e.Location) ? Cursors.Default : Cursors.Hand;
        if (hit == _hover)
            return;
        _hover = hit;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hover = null;
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.Button == MouseButtons.Left && SegmentAt(e.Location) is { } segment)
            SegmentClicked?.Invoke(segment, RectangleToScreen(_segmentBounds[segment]));
        else if (e.Button == MouseButtons.Left && OverExpandAll(e.Location))
            ExpandAllClicked?.Invoke();
        else if (e.Button == MouseButtons.Left && OverChanges(e.Location))
            ChangesClicked?.Invoke();
    }

    private StatusSegment? SegmentAt(Point p)
    {
        foreach ((StatusSegment segment, Rectangle bounds) in _segmentBounds)
        {
            if (bounds.Contains(p) && _segments.GetValueOrDefault(segment)?.Length > 0)
                return segment;
        }

        return null;
    }
}
