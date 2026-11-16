using System.Text;

namespace Notas.Editor.Formatting;

/// <summary>Opções efetivas de uma formatação (vindas das configurações e do documento).</summary>
internal sealed record SqlFormatOptions(string KeywordCase, bool OneColumnPerLine, bool BreakConditions, string Indent, string NewLine);

internal sealed partial class SqlFormatter
{
    private readonly StringBuilder _out = new();
    private readonly SqlFormatOptions _options;
    private bool _lineStart = true;
    private char _last = '\n';

    private SqlFormatter(SqlFormatOptions options) => _options = options;

    /// <summary>Começa uma linha nova com <paramref name="level"/> recuos e mais <paramref name="hang"/> espaços (alinhamento).</summary>
    private void NewLine(int level, int hang = 0)
    {
        if (!_lineStart)
            _out.Append(_options.NewLine);
        for (int i = 0; i < level; i++)
            _out.Append(_options.Indent);
        _out.Append(' ', hang);
        _lineStart = true;
    }

    private void BlankLine()
    {
        NewLine(0);
        if (_out.Length > 0 && !_out.ToString(Math.Max(0, _out.Length - 2 * _options.NewLine.Length), Math.Min(_out.Length, 2 * _options.NewLine.Length)).Equals(_options.NewLine + _options.NewLine))
            _out.Append(_options.NewLine);
    }

    /// <summary>Escreve com um espaço antes, exceto no começo da linha e depois de "(", "." e "::".</summary>
    private void Write(string text, bool spaceBefore = true)
    {
        if (!_lineStart && spaceBefore && _last is not ('(' or '.' or ':' or ' ' or '@'))
            _out.Append(' ');
        _out.Append(text);
        _last = text[^1];
        _lineStart = false;
    }
}
