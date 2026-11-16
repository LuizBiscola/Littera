using Notas.Theme;

namespace Notas.UI.Chrome;

internal sealed partial class TitleBar
{
    private const TextFormatFlags CenteredText = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
        | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;

    private string _caption = "";

    /// <summary>Texto centralizado: "arquivo.sql — Notas".</summary>
    public string Caption
    {
        get => _caption;
        set { _caption = value; Invalidate(); }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        int dpi = DeviceDpi;
        g.Clear(Palette.Background);
        using var border = new Pen(Palette.Border);
        g.DrawLine(border, 0, Height - 1, Width, Height - 1);

        Icons.Draw(g, Icons.Logo, new Rectangle(this.Px(12), this.Px(11), this.Px(18), this.Px(18)), Palette.Text, 15, dpi);
        TextRenderer.DrawText(g, "Notas", UiFonts.Semibold(13, dpi), new Point(this.Px(38), this.Px(12)), Palette.Text, TextFormatFlags.NoPadding);

        // Título centralizado na janela inteira, mas sem invadir os botões dos lados.
        int leftEnd = _left.Count > 0 ? _left[^1].Bounds.Right : this.Px(80);
        int rightStart = _actions.Count > 0 ? _actions[0].Bounds.Left : _minimize.Bounds.Left;
        int half = Math.Min(Width / 2 - leftEnd, rightStart - Width / 2) - this.Px(12);
        if (half > 0)
            TextRenderer.DrawText(g, _caption, UiFonts.Get(12, dpi), new Rectangle(Width / 2 - half, 0, half * 2, Height - 1), Palette.TextFaint, CenteredText);

        if (_actions.Count > 1)
        {
            int x = _actions[1].Bounds.Left - this.Px(3);
            g.DrawLine(border, x, this.Px(12), x, this.Px(27));
        }

        foreach (ChromeButton button in AllButtons)
            PaintButton(g, button, dpi);
    }
}
