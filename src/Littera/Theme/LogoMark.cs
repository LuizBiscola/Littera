using System.Drawing.Drawing2D;

namespace Notas.Theme;

/// <summary>
/// Logo do Notas (protótipo, quadro "Logo — Notas"): páginas empilhadas, linhas de texto e o cursor azul. Vetorial,
/// desenhada na barra de título; o ícone do exe (Assets\Notas.ico) foi gerado a partir dela.
/// </summary>
internal static class LogoMark
{
    private const float Width = 104, Height = 99;

    private static (GraphicsPath Shape, Color Fill)[]? _parts;

    /// <summary>Centralizada em <paramref name="bounds"/>, mantendo a proporção.</summary>
    public static void Draw(Graphics g, RectangleF bounds)
    {
        _parts ??= Build();
        float scale = Math.Min(bounds.Width / Width, bounds.Height / Height);
        GraphicsState state = g.Save();
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.TranslateTransform(bounds.X + (bounds.Width - Width * scale) / 2, bounds.Y + (bounds.Height - Height * scale) / 2);
        g.ScaleTransform(scale, scale);
        foreach ((GraphicsPath shape, Color fill) in _parts)
        {
            using var brush = new SolidBrush(fill);
            g.FillPath(brush, shape);
        }

        g.Restore(state);
    }

    /// <summary>Cada caminho vai para a caixa do seu nó, pelos limites reais da curva (sem os pontos de controle).</summary>
    private static (GraphicsPath, Color)[] Build() =>
        [.. LogoPaths.All.Select(p =>
        {
            var shape = new GraphicsPath();
            SvgPath.AddTo(shape, p.Data);
            RectangleF curve;
            using (var flat = (GraphicsPath)shape.Clone())
            {
                flat.Flatten(null, 0.01f);
                curve = flat.GetBounds();
            }

            using var fit = new Matrix();
            fit.Translate(p.X, p.Y);
            fit.Scale(p.Width / curve.Width, p.Height / curve.Height);
            fit.Translate(-curve.X, -curve.Y);
            shape.Transform(fit);
            return (shape, Palette.Hex(p.Color));
        })];
}
