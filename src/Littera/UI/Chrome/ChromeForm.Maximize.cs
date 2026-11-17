namespace Littera.UI.Chrome;

internal partial class ChromeForm
{
    private bool _maximizePressed;

    /// <summary>
    /// O maximizar é área não cliente (HTMAXBUTTON) para o Windows 11 mostrar o Snap Layouts.
    /// Tratamos clique e hover aqui para o DefWindowProc não desenhar o botão clássico por cima.
    /// </summary>
    private bool HandleMaximizeButton(ref Message m)
    {
        bool onButton = m.WParam == Win32.HTMAXBUTTON;
        switch (m.Msg)
        {
            case Win32.WM_NCMOUSEMOVE:
                TitleBar.MaximizeHot = onButton;
                if (onButton)
                    Win32.TrackNonClientLeave(Handle);
                return false;

            case Win32.WM_NCMOUSELEAVE:
                TitleBar.MaximizeHot = false;
                _maximizePressed = false;
                return false;

            case Win32.WM_NCLBUTTONDOWN or Win32.WM_NCLBUTTONDBLCLK when onButton:
                _maximizePressed = true;
                m.Result = IntPtr.Zero;
                return true;

            case Win32.WM_NCLBUTTONUP when onButton:
                if (_maximizePressed)
                    TitleBar.ToggleMaximize();
                _maximizePressed = false;
                m.Result = IntPtr.Zero;
                return true;

            default:
                return false;
        }
    }
}
