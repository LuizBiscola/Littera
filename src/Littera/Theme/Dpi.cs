namespace Littera.Theme;

/// <summary>Converte medidas do protótipo (pixels a 96 DPI) para o DPI do monitor do controle.</summary>
internal static class Dpi
{
    // "Px" e não "Scale": Control já tem um Scale(float) obsoleto que ganharia na resolução.
    public static int Px(this Control control, int px) => (int)Math.Round(px * control.DeviceDpi / 96.0);

    public static float Px(this Control control, float px) => px * control.DeviceDpi / 96f;
}
