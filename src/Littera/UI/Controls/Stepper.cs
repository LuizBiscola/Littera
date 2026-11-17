using Notas.Theme;

namespace Notas.UI.Controls;

/// <summary>Contador "− 13 +" do protótipo (tamanho da fonte, espaçamento).</summary>
internal sealed class Stepper : Control
{
    private float _value;

    public Stepper()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        Size = new Size(this.Px(96), this.Px(32));
        Cursor = Cursors.Hand;
    }

    public event Action? ValueChanged;

    public float Minimum { get; init; }

    public float Maximum { get; init; } = 100;

    public float Increment { get; init; } = 1;

    public string Format { get; init; } = "0";

    public float Value
    {
        get => _value;
        set
        {
            float clamped = MathF.Round(Math.Clamp(value, Minimum, Maximum), 2);
            if (clamped == _value)
                return;
            _value = clamped;
            Invalidate();
            ValueChanged?.Invoke();
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.X < this.Px(30))
            Value -= Increment;
        else if (e.X > Width - this.Px(30))
            Value += Increment;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.Clear(Parent?.BackColor ?? Palette.Background);
        Shapes.FillRounded(g, Palette.Border, new Rectangle(0, 0, Width, Height), this.Px(6));
        Shapes.FillRounded(g, Palette.Background, new Rectangle(1, 1, Width - 2, Height - 2), this.Px(5));
        using var line = new Pen(Palette.Border);
        g.DrawLine(line, this.Px(30), 1, this.Px(30), Height - 2);
        g.DrawLine(line, Width - this.Px(30), 1, Width - this.Px(30), Height - 2);

        Font font = UiFonts.Get(13, DeviceDpi);
        var flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding;
        TextRenderer.DrawText(g, "−", font, new Rectangle(0, 0, this.Px(30), Height), Palette.TextMuted, flags);
        TextRenderer.DrawText(g, "+", font, new Rectangle(Width - this.Px(30), 0, this.Px(30), Height), Palette.TextMuted, flags);
        TextRenderer.DrawText(g, _value.ToString(Format), font, Rectangle.FromLTRB(this.Px(30), 0, Width - this.Px(30), Height), Palette.Text, flags);
    }
}
