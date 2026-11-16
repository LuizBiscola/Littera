using Notas.Theme;

namespace Notas.UI;

internal sealed partial class StatusBar
{
    private int _unsaved, _saved;
    private Rectangle _changesBounds;

    /// <summary>Clique em "4 linhas não salvas · 2 salvas": vai para a próxima alteração.</summary>
    public event Action? ChangesClicked;

    public void SetChanges(int unsaved, int saved)
    {
        if (_unsaved == unsaved && _saved == saved)
            return;
        (_unsaved, _saved) = (unsaved, saved);
        Invalidate();
    }

    /// <summary>Amostra laranja com as não salvas e verde com as salvas, como no protótipo.</summary>
    private int PaintChanges(Graphics g, Font font, int x, int limit)
    {
        _changesBounds = Rectangle.Empty;
        if (_unsaved + _saved == 0 || x >= limit)
            return x;

        int start = x;
        if (_unsaved > 0)
            x = PaintSwatch(g, font, x, limit, Palette.ChangeUnsaved, _unsaved == 1 ? "1 linha não salva" : $"{_unsaved} linhas não salvas");
        if (_saved > 0)
            x = PaintSwatch(g, font, x, limit, Palette.ChangeSaved, _saved == 1 ? "1 salva" : $"{_saved} salvas");
        _changesBounds = Rectangle.FromLTRB(start, 0, x - this.Px(12), Height);
        return x + this.Px(6);
    }

    private int PaintSwatch(Graphics g, Font font, int x, int limit, Color color, string text)
    {
        if (x >= limit)
            return x;
        using (var swatch = new SolidBrush(color))
            g.FillRectangle(swatch, x, (Height - this.Px(12)) / 2, this.Px(3), this.Px(12));
        return PaintText(g, text, font, Palette.TextMuted, x + this.Px(9), limit) - this.Px(6);
    }

    private bool OverChanges(Point p) => !_changesBounds.IsEmpty && _changesBounds.Contains(p);
}
