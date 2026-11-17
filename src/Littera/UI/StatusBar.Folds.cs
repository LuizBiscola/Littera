using Littera.Theme;

namespace Littera.UI;

internal sealed partial class StatusBar
{
    private const string ExpandAllText = "Expandir tudo";

    private int _folded;
    private Rectangle _expandAllBounds;

    /// <summary>Clique em "Expandir tudo", ao lado de "N blocos recolhidos".</summary>
    public event Action? ExpandAllClicked;

    public void SetFolded(int count)
    {
        if (_folded == count)
            return;
        _folded = count;
        Invalidate();
    }

    /// <summary>Ícone, "2 blocos recolhidos" e o link "Expandir tudo", como no protótipo; devolve onde o próximo item começa.</summary>
    private int PaintFolded(Graphics g, Font font, int x, int limit)
    {
        _expandAllBounds = Rectangle.Empty;
        if (_folded == 0 || x >= limit)
            return x;

        Icons.Draw(g, Icons.CollapseAll, new Rectangle(x, 0, this.Px(12), Height), Palette.TextMuted, 10, DeviceDpi);
        string text = _folded == 1 ? "1 bloco recolhido" : $"{_folded} blocos recolhidos";
        x = PaintText(g, text, font, Palette.TextMuted, x + this.Px(18), limit) - this.Px(12);
        int width = TextRenderer.MeasureText(g, ExpandAllText, font, Size.Empty, Measure).Width;
        if (x + width > limit)
            return x + this.Px(12);

        _expandAllBounds = new Rectangle(x, 0, width, Height);
        TextRenderer.DrawText(g, ExpandAllText, font, _expandAllBounds, Palette.Accent, Plain);
        return x + width + this.Px(18);
    }

    private bool OverExpandAll(Point p) => !_expandAllBounds.IsEmpty && _expandAllBounds.Contains(p);
}
