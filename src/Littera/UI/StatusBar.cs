using Littera.Theme;

namespace Littera.UI;

internal enum StatusSegment
{
    Indent,
    Encoding,
    LineEnding,
    Language,
}

/// <summary>Barra de status do protótipo: posição, bloco atual e ocorrências à esquerda; formato à direita (clicável).</summary>
internal sealed partial class StatusBar : Control
{
    private static readonly StatusSegment[] RightSegments =
        [StatusSegment.Language, StatusSegment.LineEnding, StatusSegment.Encoding, StatusSegment.Indent];

    private readonly Dictionary<StatusSegment, string> _segments = [];
    private readonly Dictionary<StatusSegment, Rectangle> _segmentBounds = [];
    private string? _position;
    private string? _block;
    private string? _occurrences;

    public StatusBar()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        Dock = DockStyle.Bottom;
        Height = this.Px(28);
    }

    public void SetPosition(int line, int column) => Set(ref _position, $"Ln {line}, Col {column}");

    /// <summary>"Instrução 2 de 3 · linhas 6–26"; null esconde.</summary>
    public void SetBlock(string? text) => Set(ref _block, text);

    /// <summary>"id_cliente · 7 ocorrências"; null esconde.</summary>
    public void SetOccurrences(string? text) => Set(ref _occurrences, text);

    public void SetSegment(StatusSegment segment, string text)
    {
        if (_segments.TryGetValue(segment, out string? current) && current == text)
            return;
        _segments[segment] = text;
        Invalidate();
    }

    private void Set(ref string? field, string? value)
    {
        if (field == value)
            return;
        field = value;
        Invalidate();
    }

    protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
    {
        base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        Height = this.Px(28);
    }
}
