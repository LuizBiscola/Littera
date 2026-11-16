using System.Drawing.Drawing2D;

namespace Notas.Theme;

/// <summary>Converte o atributo "d" de um caminho SVG para <see cref="GraphicsPath"/> (comandos M L H V C S Q Z).</summary>
internal static partial class SvgPath
{
    public static void AddTo(GraphicsPath path, string d)
    {
        var reader = new Reader(d);
        PointF current = default, start = default, control = default;
        char command = ' ', previous = ' ';
        while (reader.More)
        {
            if (reader.AtCommand)
                command = reader.Command();
            else if (command is 'M' or 'm')
                command = command == 'M' ? 'L' : 'l'; // pares depois do M são linhas
            else if (command is ' ' or 'Z' or 'z')
                throw new FormatException("Número sem comando no caminho.");

            bool relative = char.IsLower(command);
            PointF origin = current;
            PointF Next()
            {
                PointF p = reader.Point();
                return relative ? new PointF(origin.X + p.X, origin.Y + p.Y) : p;
            }

            PointF end;
            switch (char.ToUpperInvariant(command))
            {
                case 'M':
                    path.StartFigure();
                    current = start = Next();
                    break;
                case 'L':
                    path.AddLine(current, current = Next());
                    break;
                case 'H':
                    float x = reader.Number();
                    path.AddLine(current, current = new PointF(relative ? current.X + x : x, current.Y));
                    break;
                case 'V':
                    float y = reader.Number();
                    path.AddLine(current, current = new PointF(current.X, relative ? current.Y + y : y));
                    break;
                case 'C' or 'S':
                    PointF c1 = command is 'C' or 'c' ? Next() : previous is 'C' or 'S' ? Mirror(control, current) : current;
                    control = Next();
                    path.AddBezier(current, c1, control, end = Next());
                    current = end;
                    break;
                case 'Q':
                    PointF q = Next();
                    end = Next();
                    path.AddBezier(current, Lerp(current, q), Lerp(end, q), end);
                    current = end;
                    break;
                case 'Z':
                    path.CloseFigure();
                    current = start;
                    break;
                default:
                    throw new FormatException($"Comando não suportado: '{command}'.");
            }

            previous = char.ToUpperInvariant(command);
        }
    }

    private static PointF Mirror(PointF point, PointF center) => new(2 * center.X - point.X, 2 * center.Y - point.Y);

    /// <summary>Controle da cúbica equivalente à quadrática: 2/3 do caminho até o controle.</summary>
    private static PointF Lerp(PointF from, PointF control) =>
        new(from.X + 2f / 3 * (control.X - from.X), from.Y + 2f / 3 * (control.Y - from.Y));
}
