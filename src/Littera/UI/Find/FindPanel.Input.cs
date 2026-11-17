namespace Notas.UI.Find;

internal sealed partial class FindPanel
{
    private FindButton? _hover;
    private ToolTip? _toolTip;

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        FindButton? button = _buttons.FirstOrDefault(b => b.Visible && b.Bounds.Contains(e.Location));
        if (button == _hover)
            return;
        _hover = button;
        _toolTip ??= new ToolTip();
        _toolTip.SetToolTip(this, button?.ToolTip);
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
        if (e.Button == MouseButtons.Left && _hover is { } button && button.Bounds.Contains(e.Location))
            button.OnClick();
    }

    /// <summary>Enter = próxima, Shift+Enter = anterior, Esc = fechar (nas duas caixas).</summary>
    private void HandleKeys(TextBox box)
    {
        box.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Escape)
                Close();
            else if (e.KeyCode == Keys.Enter && box == _find)
                Navigate(e.Shift ? -1 : +1);
            else
                return;
            e.SuppressKeyPress = true; // sem o "bip" do Windows
        };
        box.GotFocus += (_, _) => Invalidate();
        box.LostFocus += (_, _) => Invalidate();
    }
}
