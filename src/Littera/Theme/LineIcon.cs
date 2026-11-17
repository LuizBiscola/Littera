using System.Drawing.Drawing2D;

namespace Littera.Theme;

/// <summary>Ícone de traço no formato do Lucide: caixa 24×24, traço 2, pontas e cantos arredondados.</summary>
internal sealed class LineIcon(params string[] paths)
{
    private GraphicsPath? _shape;

    /// <summary>Lido na primeira vez que aparece (desenho só na thread da interface).</summary>
    private GraphicsPath Shape => _shape ??= Build();

    public void Draw(Graphics g, RectangleF bounds, Color color)
    {
        float scale = Math.Min(bounds.Width, bounds.Height) / 24f;
        using var shape = (GraphicsPath)Shape.Clone();
        using (var fit = new Matrix(scale, 0, 0, scale, bounds.X + (bounds.Width - 24 * scale) / 2, bounds.Y + (bounds.Height - 24 * scale) / 2))
            shape.Transform(fit);
        using var pen = new Pen(color, 2 * scale) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
        GraphicsState state = g.Save();
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.Half;
        g.DrawPath(pen, shape);
        g.Restore(state);
    }

    private GraphicsPath Build()
    {
        var shape = new GraphicsPath();
        foreach (string d in paths)
            SvgPath.AddTo(shape, d);
        return shape;
    }
}
