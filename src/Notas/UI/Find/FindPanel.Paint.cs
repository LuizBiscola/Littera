using Notas.Theme;

namespace Notas.UI.Find;

internal sealed partial class FindPanel
{
    private const TextFormatFlags Centered = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
        | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.Clear(Palette.Background);
        using (var border = new Pen(Palette.Border))
            g.DrawRectangle(border, 0, 0, Width - 1, Height - 1);

        PaintBox(g, _findBox, _find.Focused);
        if (!_replaceBox.IsEmpty)
            PaintBox(g, _replaceBox, ContainsFocus && !_find.Focused);

        foreach (FindButton button in _buttons.Where(b => b.Visible))
            PaintButton(g, button);

        if (_find.Text.Length > 0)
        {
            var counter = Rectangle.FromLTRB(_findBox.Right + this.Px(8), _findBox.Top, _previous.Bounds.Left - this.Px(4), _findBox.Bottom);
            TextRenderer.DrawText(g, _session.Label, UiFonts.Get(12, DeviceDpi), counter, Palette.TextMuted,
                TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
        }
    }

    /// <summary>Caixa de texto do protótipo: borda azul quando focada.</summary>
    private void PaintBox(Graphics g, Rectangle frame, bool focused)
    {
        Shapes.FillRounded(g, focused ? Palette.Accent : Palette.Border, frame, this.Px(6));
        frame.Inflate(-1, -1);
        Shapes.FillRounded(g, Palette.Background, frame, this.Px(5));
    }

    private void PaintButton(Graphics g, FindButton button)
    {
        if (button.Toggled || button == _hover)
            Shapes.FillRounded(g, button.Toggled ? Palette.AccentSoft : Palette.ItemHover, button.Bounds, this.Px(4));

        Color color = button.Toggled ? Palette.Accent : Palette.TextMuted;
        if (button.Text is not null)
            TextRenderer.DrawText(g, button.Text, UiFonts.Get(12, DeviceDpi), button.Bounds, color, Centered);
        else
            Icons.Draw(g, button.Glyph, button.Bounds, color, 11, DeviceDpi);
    }
}
