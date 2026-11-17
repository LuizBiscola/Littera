using Notas.Theme;

namespace Notas.UI.Controls;

internal sealed partial class DarkToolTip
{
    private void OnDraw(object? sender, DrawToolTipEventArgs e)
    {
        using (var back = new SolidBrush(Palette.Tooltip))
            e.Graphics.FillRectangle(back, e.Bounds);
        (string text, string? keys) = Parts;
        Font font = UiFonts.Get(11.5f, _dpi);
        var origin = new Point(e.Bounds.X + Px(8), e.Bounds.Y + Px(5));
        TextRenderer.DrawText(e.Graphics, text, font, origin, Color.White, Flags);
        if (keys is not null)
        {
            origin.X += TextRenderer.MeasureText(text, font, Size.Empty, Flags).Width + Px(10);
            TextRenderer.DrawText(e.Graphics, keys, font, origin, Palette.TextFaint, Flags);
        }
    }
}
