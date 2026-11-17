using System.Text;

namespace Notas.Editor.Formatting;

/// <summary>Opções efetivas de uma formatação (vindas das configurações e do documento).</summary>
internal sealed record SqlFormatOptions(string KeywordCase, bool OneColumnPerLine, bool BreakConditions, string Indent, string NewLine);

internal sealed partial class SqlFormatter
{
    private readonly StringBuilder _out = new();
    private readonly SqlFormatOptions _options;
    private bool _lineStart = true;
    private bool _blank = true;
    private int _lineBegin;
    private char _last = '\n';

    private SqlFormatter(SqlFormatOptions options) => _options = options;

    /// <summary>Começa uma linha com <paramref name="level"/> recuos e mais <paramref name="hang"/> espaços (alinhamento).</summary>
    private void NewLine(int level, int hang = 0)
    {
        if (_lineStart)
            _out.Length = _lineBegin; // já no começo: só refaz o recuo
        else
            _out.Append(_options.NewLine);

        _lineBegin = _out.Length;
        for (int i = 0; i < level; i++)
            _out.Append(_options.Indent);
        _out.Append(' ', hang);
        _lineStart = true;
    }

    /// <summary>Uma linha em branco (nunca duas seguidas, nem no começo do texto).</summary>
    private void BlankLine()
    {
        if (_blank)
            return;
        NewLine(0);
        _out.Append(_options.NewLine);
        _lineBegin = _out.Length;
        _blank = true;
    }

    /// <summary>Escreve com um espaço antes, exceto no começo da linha e depois de "(", "." e "::".</summary>
    private void Write(string text, bool spaceBefore = true)
    {
        if (!_lineStart && spaceBefore && _last is not ('(' or '.' or ':'))
            _out.Append(' ');
        _out.Append(text);
        _last = text[^1];
        _lineStart = false;
        _blank = false;
    }
}
