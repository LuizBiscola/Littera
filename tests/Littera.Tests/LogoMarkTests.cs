using System.Drawing;
using Littera.Theme;

namespace Littera.Tests;

public class LogoMarkTests
{
    [Fact]
    public void Desenha_as_partes_nas_caixas_do_prototipo()
    {
        using var bitmap = new Bitmap(104, 99);
        using (Graphics g = Graphics.FromImage(bitmap))
            LogoMark.Draw(g, new RectangleF(0, 0, 104, 99));

        Assert.Equal(Palette.Hex(0x2F6FEB).ToArgb(), bitmap.GetPixel(86, 55).ToArgb()); // cursor azul
        Assert.Equal(Color.White.ToArgb(), bitmap.GetPixel(60, 85).ToArgb()); // folha da frente, abaixo das linhas
        Assert.Equal(0, bitmap.GetPixel(100, 5).A); // canto vazio: fundo transparente
    }
}
