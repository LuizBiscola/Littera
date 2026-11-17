using System.Drawing;
using Littera.Theme;

namespace Littera.Tests;

public class WordmarkTests
{
    [Fact]
    public void Escreve_o_nome_com_a_fonte_embutida_sobre_a_linha_de_base()
    {
        using var bitmap = new Bitmap(80, 30);
        using (Graphics g = Graphics.FromImage(bitmap))
            Wordmark.Draw(g, "Littera", new PointF(2, 22), 96);

        int top = int.MaxValue, bottom = -1, right = -1;
        for (int y = 0; y < bitmap.Height; y++)
            for (int x = 0; x < bitmap.Width; x++)
                if (bitmap.GetPixel(x, y).A > 128)
                    (top, bottom, right) = (Math.Min(top, y), Math.Max(bottom, y), Math.Max(right, x));

        Assert.InRange(bottom, 21, 22); // nada abaixo da linha de base: "Littera" não tem descendentes
        Assert.InRange(top, 10, 13); // L e t com ~10 px de altura a 15 px
        Assert.InRange(right, 45, 52); // largura do nome como no protótipo (~47 px)
    }
}
