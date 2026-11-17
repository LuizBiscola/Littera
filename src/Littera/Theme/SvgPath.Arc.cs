using System.Drawing.Drawing2D;

namespace Notas.Theme;

internal static partial class SvgPath
{
    /// <summary>Arco elíptico do SVG em cúbicas de até 90° (conversão de pontas para centro, SVG 1.1 apêndice F.6).</summary>
    private static void Arc(GraphicsPath path, PointF from, float rx, float ry, float angle, bool large, bool sweep, PointF to)
    {
        if (from == to)
            return;
        if (rx == 0 || ry == 0)
        {
            path.AddLine(from, to);
            return;
        }

        double a = Math.Abs(rx), b = Math.Abs(ry), phi = angle * Math.PI / 180, cos = Math.Cos(phi), sin = Math.Sin(phi);
        double dx = (from.X - to.X) / 2.0, dy = (from.Y - to.Y) / 2.0;
        double x1 = cos * dx + sin * dy, y1 = -sin * dx + cos * dy;
        double fit = x1 * x1 / (a * a) + y1 * y1 / (b * b);
        if (fit > 1)
            (a, b) = (a * Math.Sqrt(fit), b * Math.Sqrt(fit)); // raio pequeno demais: cresce até alcançar o fim

        double numerator = a * a * b * b - a * a * y1 * y1 - b * b * x1 * x1;
        double root = (large == sweep ? -1 : 1) * Math.Sqrt(Math.Max(0, numerator / (a * a * y1 * y1 + b * b * x1 * x1)));
        double cx1 = root * a * y1 / b, cy1 = -root * b * x1 / a;
        double cx = cos * cx1 - sin * cy1 + (from.X + to.X) / 2.0, cy = sin * cx1 + cos * cy1 + (from.Y + to.Y) / 2.0;
        double start = Math.Atan2((y1 - cy1) / b, (x1 - cx1) / a);
        double delta = Math.Atan2((-y1 - cy1) / b, (-x1 - cx1) / a) - start;
        if (sweep && delta < 0)
            delta += 2 * Math.PI;
        else if (!sweep && delta > 0)
            delta -= 2 * Math.PI;

        int segments = Math.Max(1, (int)Math.Ceiling(Math.Abs(delta) / (Math.PI / 2) - 1e-6));
        double step = delta / segments, k = 4.0 / 3 * Math.Tan(step / 4);
        PointF Map(double u, double v) => new((float)(cx + a * u * cos - b * v * sin), (float)(cy + a * u * sin + b * v * cos));
        PointF point = from;
        for (int i = 0; i < segments; i++)
        {
            double t1 = start + i * step, t2 = t1 + step;
            PointF next = i == segments - 1 ? to : Map(Math.Cos(t2), Math.Sin(t2));
            path.AddBezier(point, Map(Math.Cos(t1) - k * Math.Sin(t1), Math.Sin(t1) + k * Math.Cos(t1)),
                Map(Math.Cos(t2) + k * Math.Sin(t2), Math.Sin(t2) - k * Math.Cos(t2)), next);
            point = next;
        }
    }
}
