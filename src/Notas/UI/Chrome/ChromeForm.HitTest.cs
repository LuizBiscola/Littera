using Notas.Theme;

namespace Notas.UI.Chrome;

internal partial class ChromeForm
{
    protected override void WndProc(ref Message m)
    {
        switch (m.Msg)
        {
            case Win32.WM_NCCALCSIZE when m.WParam != IntPtr.Zero:
                RemoveCaption(ref m);
                return;

            case Win32.WM_NCHITTEST:
                base.WndProc(ref m); // laterais e base: o próprio Windows responde
                if (m.Result == Win32.HTCLIENT)
                    m.Result = HitTestClient(PointToClient(Win32.PointFromLParam(m.LParam)));
                return;

            default:
                if (!HandleMaximizeButton(ref m))
                    base.WndProc(ref m);
                return;
        }
    }

    private IntPtr HitTestClient(Point p)
    {
        int edge = this.Px(5);
        if (!Win32.IsZoomed(Handle) && p.Y < edge)
        {
            if (p.X < edge * 2)
                return Win32.HTTOPLEFT;
            return p.X >= ClientSize.Width - edge * 2 ? Win32.HTTOPRIGHT : Win32.HTTOP;
        }

        if (TitleBar.MaximizeBounds.Contains(p))
            return Win32.HTMAXBUTTON;

        return p.Y < TitleBar.Height ? Win32.HTCAPTION : Win32.HTCLIENT;
    }
}
