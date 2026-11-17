using Littera.Editor;
using Littera.Theme;

namespace Littera.UI;

internal sealed partial class OpenFilesPanel
{
    private OpenDocument? _pressed, _drag;
    private Point _pressPoint, _dragPoint;
    private (bool Pinned, int Index)? _drop;

    private void BeginPress(OpenDocument document, Point point)
    {
        _pressed = document;
        _pressPoint = point;
    }

    /// <summary>Movimento com o botão apertado: passando de alguns pixels, vira arrasto.</summary>
    private bool UpdateDrag(MouseEventArgs e)
    {
        if (_pressed is null || e.Button != MouseButtons.Left)
        {
            _pressed = null;
            return false;
        }

        if (_drag is null)
        {
            if (Math.Abs(e.X - _pressPoint.X) + Math.Abs(e.Y - _pressPoint.Y) < this.Px(6))
                return true;
            _drag = _pressed;
            _hoverDocument = null;
            HideTip();
        }

        _dragPoint = e.Location;
        _drop = DropAt(e.Location);
        Invalidate();
        return true;
    }
}
