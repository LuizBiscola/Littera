namespace Notas.UI.Chrome;

internal sealed partial class TitleBar
{
    private ChromeButton? _hover;
    private ChromeButton? _pressed;

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        ChromeButton? button = ButtonAt(e.Location);
        if (button == _hover)
            return;

        _hover = button;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hover = null;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left)
            _pressed = ButtonAt(e.Location);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        ChromeButton? button = _pressed;
        _pressed = null;
        if (button is not null && button == ButtonAt(e.Location))
            button.OnClick();
    }

    private ChromeButton? ButtonAt(Point p) => AllButtons.FirstOrDefault(b => b != _maximize && b.Bounds.Contains(p));
}
