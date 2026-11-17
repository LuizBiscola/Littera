using Littera.Theme;

namespace Littera.UI.Controls;

/// <summary>Botões do rodapé do protótipo: "Salvar" escuro, "Cancelar" com borda, "Restaurar padrões" só texto.</summary>
internal sealed class FlatButton : Control
{
    private bool _hover;

    public FlatButton(string text, FlatButtonKind kind)
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        Text = text;
        Kind = kind;
        Cursor = Cursors.Hand;
        Font = UiFonts.Get(13, DeviceDpi);
        Size = new Size(TextRenderer.MeasureText(text, Font).Width + this.Px(kind == FlatButtonKind.Link ? 8 : 28), this.Px(32));
    }

    public FlatButtonKind Kind { get; }

    protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hover = true; Invalidate(); }

    protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover = false; Invalidate(); }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.Clear(Parent?.BackColor ?? Palette.Background);
        var bounds = new Rectangle(0, 0, Width, Height);
        Color text = Kind switch { FlatButtonKind.Primary => Color.White, FlatButtonKind.Link => _hover ? Palette.Text : Palette.TextMuted, _ => Palette.Text };

        if (Kind == FlatButtonKind.Primary)
            Shapes.FillRounded(g, _hover ? Palette.Hex(0x3A3F46) : Palette.Text, bounds, this.Px(6));
        else if (Kind == FlatButtonKind.Secondary)
        {
            Shapes.FillRounded(g, Palette.Border, bounds, this.Px(6));
            Shapes.FillRounded(g, _hover ? Palette.ItemHover : Palette.Background, Rectangle.Inflate(bounds, -1, -1), this.Px(5));
        }

        TextRenderer.DrawText(g, Text, Font, bounds, text, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
    }
}

internal enum FlatButtonKind
{
    Primary,
    Secondary,
    Link,
}
