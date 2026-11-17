namespace Littera.UI;

internal sealed partial class FolderPanel
{
    private int _hoverIndex = -1;
    private Rectangle _hoverButton;

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        _sizer.MouseMove(e);
        if (_sizer.Dragging)
            return;
        int index = RowAt(e.Location);
        Rectangle button = HeaderButtons().FirstOrDefault(b => b.Bounds.Contains(e.Location)).Bounds;
        if (index == _hoverIndex && button == _hoverButton)
            return;
        if (index != _hoverIndex)
            ResetMouseEventArgs(); // a dica do caminho acompanha a linha nova
        _hoverIndex = index;
        _hoverButton = button;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _sizer.MouseLeave();
        _hoverIndex = -1;
        _hoverButton = Rectangle.Empty;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        _sizer.MouseDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (_sizer.MouseUp(e))
            return;
        if (e.Button == MouseButtons.Right && RowAt(e.Location) is >= 0 and int row)
            ShowRowMenu(_rows[row].Entry, e.Location);
        if (e.Button != MouseButtons.Left)
            return;
        if (HeaderButtons().FirstOrDefault(b => b.Bounds.Contains(e.Location)).Run is { } run)
        {
            run();
            return;
        }

        int index = RowAt(e.Location);
        if (index < 0)
            return;
        if (_rows[index].Entry.IsDirectory)
            ToggleFolder(_rows[index]);
        else
            FileRequested?.Invoke(_rows[index].Entry.Path);
    }

    private (Rectangle Bounds, Action Run)[] HeaderButtons() =>
        [(RefreshButton, Reload), (CollapseButton, CollapseAll), (CloseButton, () => CloseRequested?.Invoke())];
}
