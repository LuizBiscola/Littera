using System.Runtime.InteropServices;
using Notas.Theme;
using Notas.UI.Chrome;

namespace Notas.UI.Settings;

internal sealed partial class SettingsDialog
{
    private const int CS_DROPSHADOW = 0x20000;
    private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;

    /// <summary>Sem moldura do Windows, mas com sombra.</summary>
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams parameters = base.CreateParams;
            parameters.ClassStyle |= CS_DROPSHADOW;
            return parameters;
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        int round = 2; // DWMWCP_ROUND: cantos arredondados do Windows 11
        _ = DwmSetWindowAttribute(Handle, DWMWA_WINDOW_CORNER_PREFERENCE, ref round, sizeof(int));
        Win32.SetBorderColor(Handle, Palette.Border);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode == Keys.Escape)
            Close();
    }

    /// <summary>Arrasta pelo cabeçalho (fora do X), como uma barra de título.</summary>
    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        if (m.Msg != Win32.WM_NCHITTEST || m.Result != Win32.HTCLIENT)
            return;

        Point p = _header.PointToClient(Win32.PointFromLParam(m.LParam));
        if (_header.ClientRectangle.Contains(p) && !_header.CloseBounds.Contains(p))
            m.Result = Win32.HTCAPTION;
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);
}
