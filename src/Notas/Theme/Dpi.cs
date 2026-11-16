namespace Notas.Theme;

/// <summary>Converte medidas do protótipo (pixels a 96 DPI) para o DPI do monitor do controle.</summary>
internal static class Dpi
{
    public static int Scale(this Control control, int px) => (int)Math.Round(px * control.DeviceDpi / 96.0);

    public static float Scale(this Control control, float px) => px * control.DeviceDpi / 96f;
}
