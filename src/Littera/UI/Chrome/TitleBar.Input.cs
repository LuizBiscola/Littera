using Littera.Theme;

namespace Littera.UI.Chrome;

internal sealed partial class TitleBar
{
    private ChromeButton? _hover;
    private ChromeButton? _pressed;
    private ToolTip? _toolTip;
    private bool _maximizeHot;

    /// <summary>Hover do maximizar: vem do formulário, porque ali as mensagens são de área não cliente.</summary>
    public bool MaximizeHot
    {
        get => _maximizeHot;
        set
        {
            if (_maximizeHot == value)
                return;
            _maximizeHot = value;
            Invalidate(_maximize.Bounds);
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        ChromeButton? button = ButtonAt(e.Location);
        if (button == _hover)
            return;

        _hover = button;
        _toolTip ??= new ToolTip(); // criado só no primeiro hover: nada a mais na inicialização
        _toolTip.SetToolTip(this, button?.ToolTip);
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

    /// <summary>
    /// Fora dos botões a barra fica "transparente": o formulário recebe o teste e responde
    /// HTCAPTION (arrastar, duplo clique, Aero Snap), HTMAXBUTTON ou HTTOP (redimensionar).
    /// </summary>
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == Win32.WM_NCHITTEST)
        {
            Point p = PointToClient(Win32.PointFromLParam(m.LParam));
            bool resizeEdge = p.Y < this.Px(4) && FindForm()?.WindowState != FormWindowState.Maximized;
            if (resizeEdge || ButtonAt(p) is null)
            {
                m.Result = Win32.HTTRANSPARENT;
                return;
            }
        }

        base.WndProc(ref m);
    }

    private ChromeButton? ButtonAt(Point p) => AllButtons.FirstOrDefault(b => b != _maximize && b.Bounds.Contains(p));
}
