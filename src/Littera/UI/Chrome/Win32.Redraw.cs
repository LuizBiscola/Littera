using System.Runtime.InteropServices;

namespace Notas.UI.Chrome;

internal static partial class Win32
{
    private const int WM_SETREDRAW = 0x000B;
    private const uint RDW_INVALIDATE = 0x0001;
    private const uint RDW_ERASE = 0x0004;
    private const uint RDW_ALLCHILDREN = 0x0080;
    private const uint RDW_UPDATENOW = 0x0100;
    private const uint RDW_FRAME = 0x0400;

    /// <summary>Repinta já a janela (fundo e barra de rolagem) e todos os filhos, sem esperar o WM_PAINT de cada um.</summary>
    public static void RedrawNow(IntPtr hwnd) =>
        RedrawWindow(hwnd, IntPtr.Zero, IntPtr.Zero, RDW_INVALIDATE | RDW_ERASE | RDW_FRAME | RDW_ALLCHILDREN | RDW_UPDATENOW);

    /// <summary>Pinta já o que está pendente na janela e nos filhos (não invalida nada a mais).</summary>
    public static void UpdateNow(IntPtr hwnd) => RedrawWindow(hwnd, IntPtr.Zero, IntPtr.Zero, RDW_ALLCHILDREN | RDW_UPDATENOW);

    /// <summary>Desligado, o que está na tela fica parado enquanto os filhos são trocados; religar não repinta sozinho.</summary>
    public static void SetRedraw(IntPtr hwnd, bool on) => SendMessage(hwnd, WM_SETREDRAW, on ? 1 : 0, IntPtr.Zero);

    /// <summary>
    /// WS_EX_COMPOSITED: o Windows pinta a janela e todos os filhos fora da tela e mostra o resultado de uma vez
    /// (sem meio-termo visível). Ligado só enquanto precisa: deixa cada repintura mais cara.
    /// </summary>
    public static void SetComposited(IntPtr hwnd, bool on)
    {
        long style = GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();
        long updated = on ? style | WS_EX_COMPOSITED : style & ~WS_EX_COMPOSITED;
        if (updated != style)
            SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(updated));
    }

    private const int GWL_EXSTYLE = -20;
    private const long WS_EX_COMPOSITED = 0x02000000;

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")] private static extern IntPtr GetWindowLongPtr(IntPtr hwnd, int index);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW")] private static extern IntPtr SetWindowLongPtr(IntPtr hwnd, int index, IntPtr value);

    [DllImport("user32.dll")] private static extern bool RedrawWindow(IntPtr hwnd, IntPtr rect, IntPtr region, uint flags);

    [DllImport("user32.dll")] private static extern IntPtr SendMessage(IntPtr hwnd, int msg, nint wParam, IntPtr lParam);
}
