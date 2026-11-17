using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace Notas.Theme;

/// <summary>
/// O nome escrito na fonte da identidade (protótipo, opção A "Clássico": Fraunces 600, 15 px). A Fraunces vai embutida
/// (Assets\Fonts\LitteraFraunces.ttf: a instância que o pen.dev usa, peso 600 e tamanho óptico 15, só latim) e é
/// carregada só para o app, sem instalar nada no Windows.
/// </summary>
internal static class Wordmark
{
    private const float SizePx = 15;

    private static PrivateFontCollection? _fonts;

    private static FontFamily Family => (_fonts ??= Load()).Families[0];

    /// <summary>Escreve com a linha de base em <paramref name="baseline"/>.Y, a caixa do texto começando em .X.</summary>
    public static void Draw(Graphics g, string text, PointF baseline, int dpi)
    {
        FontFamily family = Family;
        float em = SizePx * dpi / 96f;
        float ascent = em * family.GetCellAscent(FontStyle.Regular) / family.GetEmHeight(FontStyle.Regular);
        using var font = new Font(family, em, FontStyle.Regular, GraphicsUnit.Pixel);
        using var brush = new SolidBrush(Palette.Text);
        GraphicsState state = g.Save();
        g.TextRenderingHint = TextRenderingHint.AntiAlias; // tons de cinza, como no pen.dev
        g.DrawString(text, font, brush, baseline.X, baseline.Y - ascent, StringFormat.GenericTypographic);
        g.Restore(state);
    }

    private static PrivateFontCollection Load()
    {
        using Stream stream = typeof(Wordmark).Assembly.GetManifestResourceStream("LitteraFraunces.ttf")!;
        byte[] data = new byte[stream.Length];
        stream.ReadExactly(data);
        IntPtr memory = Marshal.AllocCoTaskMem(data.Length); // fica com o app: a coleção lê a fonte desta memória
        Marshal.Copy(data, 0, memory, data.Length);
        var fonts = new PrivateFontCollection();
        fonts.AddMemoryFont(memory, data.Length);
        return fonts;
    }
}
