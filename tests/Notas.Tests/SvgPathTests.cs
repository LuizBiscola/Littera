using System.Drawing;
using System.Drawing.Drawing2D;
using Notas.Theme;

namespace Notas.Tests;

public class SvgPathTests
{
    private static GraphicsPath Parse(string d)
    {
        var path = new GraphicsPath();
        SvgPath.AddTo(path, d);
        return path;
    }

    [Fact]
    public void Le_numeros_colados_e_comandos_relativos()
    {
        PointF[] points = Parse("m2.305 15.53.923-.382").PathPoints;
        Assert.Equal(2, points.Length);
        Assert.Equal(new PointF(2.305f, 15.53f), points[0]);
        Assert.Equal(3.228f, points[1].X, 3);
        Assert.Equal(15.148f, points[1].Y, 3);
    }

    [Fact]
    public void Pares_depois_do_M_viram_linhas()
    {
        PointF[] points = Parse("M10 12.5 8 15l2 2.5").PathPoints;
        Assert.Equal([new PointF(10, 12.5f), new PointF(8, 15), new PointF(10, 17.5f)], points); // GDI+ junta linhas seguidas
    }

    [Fact]
    public void Arcos_formam_a_elipse_inteira()
    {
        RectangleF bounds = Parse("M3 5a9 3 0 1 0 18 0a9 3 0 1 0-18 0").GetBounds();
        Assert.Equal(3, bounds.Left, 2);
        Assert.Equal(21, bounds.Right, 2);
        Assert.Equal(2, bounds.Top, 2);
        Assert.Equal(8, bounds.Bottom, 2);
    }

    [Theory]
    [InlineData("10 10")]
    [InlineData("M1 1 X2 2")]
    [InlineData("M1 1a2 2 0 2 0 3 3")]
    public void Recusa_caminho_invalido(string d)
    {
        Assert.Throws<FormatException>(() => Parse(d));
    }
}
