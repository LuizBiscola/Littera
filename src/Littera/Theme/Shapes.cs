using System.Drawing.Drawing2D;

namespace Notas.Theme;

/// <summary>Retângulos arredondados com antialiasing (botões, abas, selos).</summary>
internal static class Shapes
{
    public static void FillRounded(Graphics g, Color color, Rectangle bounds, int radius)
    {
        using var brush = new SolidBrush(color);
        if (radius <= 0)
        {
            g.FillRectangle(brush, bounds);
            return;
        }

        using GraphicsPath path = RoundedPath(bounds, radius);
        SmoothingMode previous = g.SmoothingMode;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.FillPath(brush, path);
        g.SmoothingMode = previous;
    }

    private static GraphicsPath RoundedPath(Rectangle r, int radius)
    {
        int d = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
        var path = new GraphicsPath();
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}
