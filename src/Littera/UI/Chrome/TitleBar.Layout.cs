using Littera.Theme;

namespace Littera.UI.Chrome;

internal sealed partial class TitleBar
{
    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        int size = this.Px(28), top = this.Px(6), gap = this.Px(2);

        _brand.Bounds = new Rectangle(this.Px(6), top, this.Px(86), size); // logo + "Littera"
        int x = this.Px(97);
        foreach (ChromeButton button in _left)
        {
            button.Bounds = new Rectangle(x, top, size, size);
            x += size + gap;
        }

        int right = Width, windowButton = this.Px(46);
        foreach (ChromeButton button in new[] { _close, _maximize, _minimize })
        {
            right -= windowButton;
            button.Bounds = new Rectangle(right, 0, windowButton, Height);
        }

        right -= this.Px(10);
        for (int i = _actions.Count - 1; i >= 0; i--)
        {
            right -= size;
            _actions[i].Bounds = new Rectangle(right, top, size, size);
            right -= i == 1 ? this.Px(5) : gap; // espaço do divisor entre a busca e as demais ações
        }
    }

    protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
    {
        base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        Height = this.Px(40);
    }
}
