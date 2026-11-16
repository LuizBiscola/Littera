namespace Notas.UI.Chrome;

internal static partial class Win32
{
    private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
    private const int DWMWCP_ROUND = 2;

    /// <summary>Windows 11 arredonda janelas suspensas e dá a sombra delas; no 10 os menus ficam retos com borda própria.</summary>
    public static bool IsWindows11 { get; } = Environment.OSVersion.Version.Build >= 22000;

    /// <summary>Cantos arredondados (com sombra do sistema) e borda na cor pedida; no Windows 10 não faz nada.</summary>
    public static void RoundCorners(IntPtr hwnd, Color border)
    {
        if (!IsWindows11)
            return;
        int round = DWMWCP_ROUND;
        _ = DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref round, sizeof(int));
        int colorRef = ColorTranslator.ToWin32(border);
        _ = DwmSetWindowAttribute(hwnd, DWMWA_BORDER_COLOR, ref colorRef, sizeof(int));
    }
}
