using System.Runtime.InteropServices;

namespace Notas.UI.Chrome;

/// <summary>Mensagens e funções do Windows usadas para desenhar a barra de título no lugar do sistema.</summary>
internal static partial class Win32
{
    public const int WM_NCCALCSIZE = 0x0083;
    public const int WM_NCHITTEST = 0x0084;
    public const int WM_NCMOUSEMOVE = 0x00A0;
    public const int WM_NCLBUTTONDOWN = 0x00A1;
    public const int WM_NCLBUTTONUP = 0x00A2;
    public const int WM_NCLBUTTONDBLCLK = 0x00A3;
    public const int WM_NCMOUSELEAVE = 0x02A2;

    public const int HTTRANSPARENT = -1;
    public const int HTCLIENT = 1;
    public const int HTCAPTION = 2;
    public const int HTMAXBUTTON = 9;
    public const int HTTOP = 12;
    public const int HTTOPLEFT = 13;
    public const int HTTOPRIGHT = 14;

    private const int SM_CYSIZEFRAME = 33;
    private const int SM_CXPADDEDBORDER = 92;
    private const int DWMWA_BORDER_COLOR = 34;

    /// <summary>Espessura da borda de redimensionamento (que o Windows deixa fora da tela ao maximizar).</summary>
    public static int ResizeFrame(int dpi) =>
        GetSystemMetricsForDpi(SM_CYSIZEFRAME, (uint)dpi) + GetSystemMetricsForDpi(SM_CXPADDEDBORDER, (uint)dpi);

    /// <summary>Coordenadas de tela empacotadas no lParam das mensagens de mouse (com sinal: monitores à esquerda).</summary>
    public static Point PointFromLParam(IntPtr lParam)
    {
        int value = unchecked((int)(long)lParam);
        return new Point((short)(value & 0xFFFF), (short)((value >> 16) & 0xFFFF));
    }

    public static void SetBorderColor(IntPtr hwnd, Color color)
    {
        int colorRef = color.R | (color.G << 8) | (color.B << 16);
        _ = DwmSetWindowAttribute(hwnd, DWMWA_BORDER_COLOR, ref colorRef, sizeof(int));
    }

    public static void TrackNonClientLeave(IntPtr hwnd)
    {
        var track = new TRACKMOUSEEVENT { cbSize = Marshal.SizeOf<TRACKMOUSEEVENT>(), dwFlags = 0x12, hwndTrack = hwnd };
        TrackMouseEvent(ref track); // 0x12 = TME_LEAVE | TME_NONCLIENT
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct TRACKMOUSEEVENT { public int cbSize; public uint dwFlags; public IntPtr hwndTrack; public uint dwHoverTime; }

    [DllImport("user32.dll")] public static extern bool IsZoomed(IntPtr hwnd);
    [DllImport("user32.dll")] private static extern int GetSystemMetricsForDpi(int index, uint dpi);
    [DllImport("user32.dll")] private static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT track);
    [DllImport("dwmapi.dll")] private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);
}
