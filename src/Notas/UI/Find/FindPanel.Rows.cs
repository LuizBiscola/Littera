using Notas.Theme;

namespace Notas.UI.Find;

internal sealed partial class FindPanel
{
    private Rectangle _replaceBox; // vazio quando a linha de substituir está escondida

    partial void LayoutRows()
    {
        int y = _findBox.Bottom + this.Px(8);
        _expand!.Bounds = new Rectangle(this.Px(6), _findBox.Y + this.Px(3), this.Px(24), this.Px(24));

        _replaceBox = Rectangle.Empty;
        if (_replaceShown)
        {
            _replaceBox = new Rectangle(_findBox.X, y, _findBox.Width, _findBox.Height);
            PlaceText(_replace!, _replaceBox, this.Px(16));
            _replaceOne!.Bounds = new Rectangle(_replaceBox.Right + this.Px(8), y + this.Px(3), this.Px(70), this.Px(24));
            _replaceAll!.Bounds = new Rectangle(_replaceOne.Bounds.Right + this.Px(4), y + this.Px(3), this.Px(50), this.Px(24));
            y = _replaceBox.Bottom + this.Px(8);
        }

        // "Em: Arquivo | Bloco atual | Seleção"; abas abertas e pasta ficam para a Fase 2.
        int x = _findBox.X + this.Px(26);
        foreach (FindButton scope in _scopes)
        {
            int width = TextRenderer.MeasureText(scope.Text, UiFonts.Get(12, DeviceDpi)).Width + this.Px(4);
            scope.Bounds = new Rectangle(x, y, width, this.Px(22));
            x += width + this.Px(6);
        }

        int height = y + this.Px(30);
        if (Height != height)
            Height = height;
    }
}
