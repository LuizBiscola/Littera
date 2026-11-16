using System.Drawing.Drawing2D;

namespace Notas.Theme;

/// <summary>Converte o atributo "d" de um caminho SVG para <see cref="GraphicsPath"/> (comandos M L H V Z).</summary>
internal static partial class SvgPath
{
    public static void AddTo(GraphicsPath path, string d)
    {
        var reader = new Reader(d);
        PointF current = default, start = default;
        char command = ' ';
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
                case 'Z':
                    path.CloseFigure();
                    current = start;
                    break;
                default:
                    throw new FormatException($"Comando não suportado: '{command}'.");
            }
        }
    }
}
