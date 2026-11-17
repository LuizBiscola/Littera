using System.Globalization;

namespace Littera.Theme;

internal static partial class SvgPath
{
    /// <summary>Lê comandos e números do atributo "d", inclusive na forma compacta ("15.53.923-.382" = 15.53, .923, -.382).</summary>
    private sealed class Reader(string text)
    {
        private int _index;

        public bool More
        {
            get
            {
                Skip();
                return _index < text.Length;
            }
        }

        public bool AtCommand => More && char.IsLetter(text[_index]) && text[_index] is not ('e' or 'E');

        public char Command() => More ? text[_index++] : throw new FormatException("Fim inesperado do caminho.");

        /// <summary>Flags de arco podem vir coladas ("0 011 4 0"): um caractere só.</summary>
        public bool Flag() => Command() switch
        {
            '0' => false,
            '1' => true,
            char c => throw new FormatException($"Flag de arco inválida: '{c}'."),
        };

        public PointF Point() => new(Number(), Number());

        public float Number()
        {
            Skip();
            int start = _index;
            if (_index < text.Length && text[_index] is '+' or '-')
                _index++;
            bool dot = false;
            while (_index < text.Length)
            {
                char c = text[_index];
                if (c == '.' && !dot)
                    dot = true;
                else if (c is 'e' or 'E')
                {
                    if (_index + 1 < text.Length && text[_index + 1] is '+' or '-')
                        _index++;
                    dot = true; // depois do expoente, outro ponto começa outro número
                }
                else if (!char.IsAsciiDigit(c))
                    break;
                _index++;
            }

            return start == _index
                ? throw new FormatException($"Número esperado na posição {start}.")
                : float.Parse(text.AsSpan(start, _index - start), NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        private void Skip()
        {
            while (_index < text.Length && (char.IsWhiteSpace(text[_index]) || text[_index] == ','))
                _index++;
        }
    }
}
